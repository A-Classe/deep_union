using System;
using UnityEngine;

namespace Module.Player.Controller
{
    public class PlayerStatusVisualizer : MonoBehaviour
    {
        [SerializeField] private Renderer lifeRenderer;
        [SerializeField] private Material lifeMaterial;
        private static readonly int LifeColorTime = Shader.PropertyToID("LifeColorTime");

        private void Start()
        {
            lifeMaterial = lifeRenderer.GetSharedMaterials();
        }

        public void SetHpRate(float rate)
        {
            lifeMaterial.SetFloat(LifeColorTime, rate);
        }
    }
}