using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Module.GameSetting
{
    [RequireComponent(typeof(Volume))]
    public class BrightController: MonoBehaviour
    {
        private Volume globalVolume;
        
        private readonly float exposureDefault = 1.18f;
        private readonly float exposureMin = -7.0f;
        private readonly float exposureMax = 4.0f;

        private void Awake()
        {
            globalVolume = GetComponent<Volume>();
            if (globalVolume.profile.TryGet(out ColorAdjustments colorAdjustments))
            {
                Debug.Log(colorAdjustments.postExposure.value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value">0.0~1.0</param>
        public void SetBrightness(float value)
        {
            if (value < 0.0f || value > 1.0f) return;
            if (globalVolume.profile.TryGet(out ColorAdjustments colorAdjustments))
            {
                float remappedValue;
                if (value <= 0.6f)
                {
                    // 0.0から0.6の範囲を exposureMin から exposureDefault にマッピング
                    remappedValue = Remap(value, 0.0f, 0.6f, exposureMin, exposureDefault);
                }
                else
                {
                    // 0.6から1.0の範囲を exposureDefault から exposureMax にマッピング
                    remappedValue = Remap(value, 0.6f, 1.0f, exposureDefault, exposureMax);
                }
                colorAdjustments.postExposure.value = remappedValue;
            }
        }

        private float Remap(float value, float from1, float to1, float from2, float to2)
        {
            return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
        }
    }
}