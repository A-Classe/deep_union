using DG.Tweening;
using UnityEngine;

namespace Module.Player.Controller
{
    public class PinAssistantVisualizer : MonoBehaviour
    {
        private static readonly int GradationTime = Shader.PropertyToID("_GradationTime");
        [SerializeField] private float maxGradationTime = 1.5f;
        [SerializeField] private Renderer gradationRenderer;
        private Material gradationMaterial;
        private Tween gradationTween;

        private void Start()
        {
            gradationMaterial = gradationRenderer.sharedMaterial;
            gradationMaterial.SetFloat(GradationTime, maxGradationTime);
        }

        public void StartGradation(float duration)
        {
            gradationTween?.Kill();
            gradationTween = gradationMaterial.DOFloat(0f, GradationTime, duration).Play();
        }

        public void StopGradation()
        {
            gradationTween?.Kill();
            gradationMaterial.SetFloat(GradationTime, maxGradationTime);
        }
    }
}