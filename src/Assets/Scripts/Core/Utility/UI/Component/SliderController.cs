using System;
using AnimationPro.RunTime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Utility.UI.Component
{
    public class SliderController : AnimationBehaviour
    {
        [SerializeField] private Slider slider;
        [SerializeField] private TextMeshProUGUI text;

        public float Value => slider.value;

        public void SetValue(float value)
        {
            Set(value);
        }

        public void Setup(float maxValue, float minValue, float value)
        {
            slider.maxValue = maxValue;
            slider.minValue = minValue;
            Set(value);
        }

        private void Set(float value)
        {
            slider.value = value;
            if (text != null)
            {
                text.text = ((int)value).ToString();
            }
        }
        
        [NonSerialized] public RectTransform rectTransform;

        protected override void Awake()
        {
            base.Awake();
            rectTransform = GetComponent<RectTransform>();
        }

    }
}