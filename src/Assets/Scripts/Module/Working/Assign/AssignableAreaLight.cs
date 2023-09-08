using System;
using UnityEngine;
using Wanna.DebugEx;

namespace Module.Task
{
    public class AssignableAreaLight : MonoBehaviour
    {
        [SerializeField] private Projector areaLight;
        [SerializeField] private float softLightRadius;
        [SerializeField] private float fixOffset;

        private float areaSize;

        private void OnValidate()
        {
            AdjustAreaLight(areaSize);
        }

        public void AdjustAreaLight(float areaSize)
        {
            this.areaSize = areaSize;
            
            areaLight.spotAngle = CalcSpotAngle(areaSize);
            areaLight.innerSpotAngle = CalcSpotAngle(areaSize - softLightRadius);

            float CalcSpotAngle(float radius)
            {
                float fixedRadius = radius + fixOffset;

                var range = areaLight.range;
                var hypo = Mathf.Sqrt(range * range + fixedRadius * fixedRadius);

                //余弦定理より照射角度を求める
                var radian = Mathf.Acos((range * range + hypo * hypo - fixedRadius * fixedRadius) / (2 * range * hypo));

                return radian * Mathf.Rad2Deg * 2f;
            }
        }
    }
}