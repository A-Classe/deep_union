using UnityEngine;

namespace Module.Player.Controller
{
    public class PlayerStatusVisualizer : MonoBehaviour
    {
        private static readonly int LifeColorTime = Shader.PropertyToID("_LifeColorTime");
        [SerializeField] private Renderer lifeRenderer;
        [SerializeField] private Material lifeMaterial;

        private void Start()
        {
            lifeMaterial = lifeRenderer.sharedMaterials[1];
            SetHpRate(0f);
        }

        public void SetHpRate(float rate)
        {
            lifeMaterial.SetFloat(LifeColorTime, rate);
        }
    }
}