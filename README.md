<https://user-images.githubusercontent.com/5408480/158495833-fd7c4836-3924-4127-ac56-f29c75273958.mp4>

# Virtual Ray Tracer

Virtual Ray Tracer is a Unity application that visualizes ray tracing. The application shows a scene with a camera, lights and objects. Rays slowly shoot from the camera and interact with the lights and objects in the scene. Users can change the settings of the ray tracer and material properties of the objects and see how this affects the rays traced in real-time. The application comes with a number of scenes that walk the user through the settings and controls of the application as well as several ray tracing concepts.

## Building the Application

To build the application you need a [Unity 2020.3 LTS](https://unity3d.com/unity/qa/lts-releases) release. To build the application, open the `Unity` folder with Unity, navigate to `File > Build Settings`, select your desired platform and press 'build'. The application has been tested on Windows, Linux and MacOS. For more information on building Unity applications see the [Unity Manual page](https://docs.unity3d.com/Manual/BuildSettings.html).

## Implementation

There are three classes at the core of the application: the `SceneManager`, the `RayManager` and the `UnityRayTracer`. The `SceneManager` handles the scene data consisting of a camera, lights and objects. The `UnityRayTracer` takes this scene data and produces a set of rays. The `RayManager` is in charge of visualizing these rays.

### Scene Management

Almost all scene data relevant to the ray tracer is stored in standard Unity components. For example, the material properties of an object are stored in a `Material` component. To mark certain `GameObject`s in the Unity scene as a camera, light or mesh to be sent to the ray tracer, a custom `RTCamera`, `RTLight` or `RTMesh` component should be attached. These components each provide fields with getters and setters for easy access to properties needed for ray tracing. Beyond that, the `RTLight` and `RTMesh` components are not much more than tags to indicate to the `SceneManager` that the objects they are attached to should be sent to the ray tracer. The `RTCamera` component is slightly more complex.  It does not rely on an attached Unity `Camera` component and needs to store its own data. It also has code for drawing the camera in the scene.

At the start of a scene, the `SceneManager` finds all instances of `RTCamera`, `RTLight` and `RTMesh` components and stores them in one `RTScene` object. This `RTScene` object then serves as the input to the ray tracer. The `SceneManager` also handles the selection and deselection of objects in the scene as well as object creation and deletion.

### Ray Tracing

The ray tracer is implemented in the `UnityRayTracer` class. The ray tracing is done by using Unity's built in `Physics.Raycast` function to cast rays and determine object intersections. This function casts a ray from a given point and returns information about the first object with a `Collider` component that it intersects. Each object with an `RTMesh` component has such a `Collider` component, so the ray tracer is aware of them.

Most of the ray tracer code is relatively straightforward if you are familiar with ray tracing. It closely matches the code of the ray tracer used in the Computer Graphics course taught at the University of Groningen. The most important difference is that main output of the ray tracer is rays instead of images.

The rays are represented with simple `RTRay` objects that store the origin, direction and length of the ray. Because ray tracing follows a recursive pattern with one ray potentially resulting in many more rays being cast, we store the rays in a tree. Each recursive call to the `Trace` function builds up a `TreeNode<RTRay>` object which is then added as a child of the caller's tree. The output of the ray tracer's `Render` function is then a `List<TreeNode<RTRay>>`, one `TreeNode<RTRay>` for each pixel.

### Ray Visualization

The `RayManager` takes this list of ray trees and draws them in the scene. Most of the drawing code lives in the `RayObject` class. `RayObject`s take in an `RTRay` and they position, orient and scale a cylinder to match the ray's origin, direction and length. The `RayManager` simply manages these `RayObjects` by providing them the rays from the ray trees produced by the ray tracer. When animating the rays, the `RayManager` also informs each `RayObject` how long it needs to be. By increasing the length a bit each frame the `RayManager` can achieve the effect of rays slowly shooting from the camera into the scene. This animation is done in a recursive fashion, so first all rays at the root of their ray tree are extended, then their children, and so on.

Ideally, each `RayObject` would be paired with one `RTRay` for its entire lifetime. However, this becomes a problem when the user changes something about the scene and a new list of ray trees is produced by the ray tracer. The simplest thing to do then would be to destroy all existing `RayObject`s and instantiate new ones for the new ray trees. The problem is that this becomes too slow when the user changes a value in the scene continuously (e.g. by dragging around an object) and there are new rays being traced every frame. Instead, it is possible to reuse existing `RayObjects` by providing them with a new `RTRay` and deactivating those we do not need. This approach of having a set of objects and activating and deactivating them instead of instantiating and destroying them is called object pooling. It is implemented in the `RayObjectPool` class.

### Program structure

This application includes two different rendering methods. The ray tracer and the 3D Unity application. Originally the ray tracer was used to render the preview screen as well as the rendered image. Unity was then used to render the scenes with rasterization. In this implementation, the scenes and rendered image are rendered by the Unity application with real-time ray tracing. Because Unity allows us to implement ray tracing, we can now render the image without the need for VRT's ray tracing method.  So it simulates light behavior based on real-world physics principles considering factors like light intensity, color, distance, and materials' properties to calculate the final lighting results. VRT uses Phong's shading model, which has components like ambient, diffuse, specular, and shininess. Ambient decides the constant lighting regardless of position of the lights in the scene. Diffuse simulates the way light scatters when it interacts with a rough surface. Specular simulates the reflection of concentrated light. So this is responsible for highlighted parts of objects. Shininess mainly controls the size of the specular highlight. Unity offers metallic and smoothness components for materials.  The metallic component is used to decide how "metal-like" a surface is. When the component is higher, the environment is reflected more and its albedo color becomes less visible~\cite{metallic}. The smoothness controls the "microsurface detail" controlling how light scatters. This differs a lot from VRT's model so that is also a reason to let Unity render the final image.

### Anamorphic Sculptures Scenes

In this version of VRT, three new levels were added to the application. These levels have the educational purpose of teaching users of VRT how to build anamorphic sculptures. The scenes were built by using the reflection level and tweaking it to display deformed objects whose reflection on one of three different types of mirrors (plane, cylinder, or sphere) displays a distinguishable object. All of the mirrors were built using prefabs and mirror textures already available in the application. To add the deformed sculptures to the scene, we uploaded their meshes to Unity and created prefabs using the `RTMesh` and specular textures present in the project. The introduction panels of the levels explain the deformation process and interaction with the objects in the scene is limited to only being able to change the colour, size, and position of the objects situated in front of the `RTCamera`. 

The deformed meshes used in this project are constructed separately from VRT using a [C++ framework](https://github.com/irinaB11/anamorphicRayTracer/tree/main). This framework was originally developed for the students of Computer Graphics at the University of Groningen. Since its initial use, it has been modified to deform the meshes of objects based on their reflections on a mirror. 

Right now, the levels are limited to only showing how deforming an object based on reflection works. We hope that in the future, deforming an object using its refraction through an optic media will be added. Another possible development would be to make it possible to deform the object directly in VRT, eliminating the necessity of a separate framework.

### Further Details

This covers all the most important components of the application, but there are many more small details. Please look through the source code if you want to know more, everything is well documented.

## About us

Virtual Ray Tracer was created by Chris van Wezel and Willard Verschoore as a graduation project for their Computing Science Bachelor's degree programme at the University of Groningen. The real-time ray tracing extension was created by Tomas de Vries as a graduation project for his Computing Science Bachelor's degree programme. Both projects were proposed and supervised by Jiri Kosinka and Steffen Frey in the [SVCG research group](https://www.cs.rug.nl/svcg/Main/HomePage). The application was built to aid students of the Computer Graphics course at the University of Groningen by providing them with an interactive introduction to the principles of ray tracing. A user study conducted at the end of the project found positive results. This success inspired the supervisors of the project to write a paper in collaboration with Chris and Willard. The paper is about to appear as an educational paper at the Eurographics 2022 conference and the EG Digital Library (DOI to follow soon).


## Future

The application is still in active development. A new user study has been conducted and, under the guidance of the same supervisors, a new group of students started their Bachelor's projects focused on improving and extending the application. And we hope that you will contribute to Virtual Ray Tracer, too; read on.

## License

The application is released under the MIT license. Therefore, you may use and modify the code as you see fit. If you use or build on the application we would appreciate it if you cited this repository and the Eurographics 2022 paper. As we are still working on the application ourselves, we would also like to hear about any improvements you may have made.

## Contact

Any questions, bug reports or suggestions can be created as an issue on this repository. Alternatively, please contact [Jiri Kosinka](http://www.cs.rug.nl/svcg/People/JiriKosinka).

## Paper and theses

W.A. Verschoore de la Houssaije, C.S. van Wezel, S. Frey, and J. Kosinka: Virtual Ray Tracer, Eurographics 2022 Education Papers; <https://diglib.eg.org/handle/10.2312/eged20221045>.

W.A. Verschoore de la Houssaije: A Virtual Ray Tracer, BSc thesis, University of Groningen, 2021; <http://fse.studenttheses.ub.rug.nl/24859>.

C.S. van Wezel: A Virtual Ray Tracer, BSc thesis, University of Groningen, 2022; <http://fse.studenttheses.ub.rug.nl/26455>.

Further documents will appear here in due course.
