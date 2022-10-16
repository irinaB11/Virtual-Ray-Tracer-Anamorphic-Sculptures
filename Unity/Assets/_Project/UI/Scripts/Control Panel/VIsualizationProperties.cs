using _Project.Ray_Tracer.Scripts;
using System;
using System.Collections.Generic;
using TMPro;
using TreeEditor;
using UnityEngine;
using _Project.Ray_Tracer.Scripts.RT_Ray;
using _Project.Ray_Tracer.Scripts.Utility;

namespace _Project.UI.Scripts.Control_Panel
{
    public class VisualizationProperties : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI resetText;
        [SerializeField]
        private TextMeshProUGUI startLoopText;
/*        [SerializeField]
        private TextMeshProUGUI noCollisionText;
        [SerializeField]
        private TextMeshProUGUI computeRayText;
        [SerializeField]
        private TextMeshProUGUI castChildrenText;
        [SerializeField]
        private TextMeshProUGUI stopLoopText;

        //[SerializeField]
        private List<TextMeshProUGUI> steps;*/

        private RayManager rayManager;
        private TreeNode<RTRay> ray;

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        private void Awake()
        {
            Debug.Log("visprops awake");
/*            steps = new List<TextMeshProUGUI>
            {
                resetText,
                startLoopText,
                noCollisionText,
                computeRayText,
                castChildrenText,
                stopLoopText
            };*/
        }

        private void Start()
        {
            rayManager = RayManager.Get();
            if (rayManager.HasSelectedRay)
                ray = rayManager.SelectedRay;

        }

        /*        public void onStepChange(int prevStep, int newStep)
                {
                    if (prevStep >= 0)
                        steps[prevStep].color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
                    steps[newStep].color = new Color(0.0f, 1.0f, 1.0f, 1.0f);
                }*/
    }
}
