using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Module.Player.Controller
{
    public class PinAssistantVisualizer : MonoBehaviour
    {
        private static readonly int gradationTime = Shader.PropertyToID("_GradationTime");
        [SerializeField] private Renderer GradationRenderer;
        private Material gradationMaterial;

        void Start()
        {
            gradationMaterial = GradationRenderer.sharedMaterial;
            setGradationRate(1.5f);
        }

        public void setGradationRate(float rate)
        {
            gradationMaterial.SetFloat(gradationTime, rate);
        }
    }

}
