using System;
using System.Collections.Generic;
using UnityEngine;

namespace Module.Player.Controller
{
    public class PlayerStatusVisualizer : MonoBehaviour
    {
        [SerializeField] private Renderer lifeRenderer;
        [SerializeField] private Material lifeMaterial;
        private static readonly int LifeColorTime = Shader.PropertyToID("_LifeColorTime");

        private void Start()
        {
            lifeMaterial = lifeRenderer.sharedMaterials[1];
        }

        public void SetHpRate(float rate)
        {
            lifeMaterial.SetFloat(LifeColorTime, rate);
        }
    }
}