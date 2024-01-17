using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Module.Assignment.Component
{
    /// <summary>
    /// AssignableAreaのライトを制御するクラス
    /// </summary>
    public class AssignableAreaLight : MonoBehaviour
    {
        [SerializeField] private DecalProjector lightProjector;
        [SerializeField] private float intensity = 1f;
        public float Intensity => intensity;

        private Material lightMaterial;
        private static readonly int IntensityKey = Shader.PropertyToID("_Intensity");

        private void Awake()
        {
            //Decalだと自動で複製されないので新しいインスタンスを作る
            var newMaterial = new Material(lightProjector.material);
            lightProjector.material = newMaterial;
            lightMaterial = newMaterial;

            SetIntensity(intensity);
        }

        public void SetIntensity(float intensity)
        {
            lightMaterial.SetFloat(IntensityKey, intensity);
        }

        public void SetLightSize(float2 size)
        {
            lightProjector.size = new Vector3(size.x, size.y, 10f);
        }
    }
}