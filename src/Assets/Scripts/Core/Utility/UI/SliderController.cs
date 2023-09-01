﻿using System;
using AnimationPro.RunTime;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Core.Utility.UI
{
    public class SliderController : AnimationBehaviour
    {
        [SerializeField] private Slider slider;
        [SerializeField] private TextMeshProUGUI text;

        public float Value => slider.value;

        public void SetValue(float value)
        {
            slider.value = value;
            if (text != null)
            {
                text.text = ((int)value).ToString();
            }
        }

        public void Setup(float maxValue, float minValue, float value)
        {
            slider.maxValue = maxValue;
            slider.minValue = minValue;
            slider.value = value;
        }
        
        [NonSerialized] public RectTransform rectTransform;

        protected override void Awake()
        {
            base.Awake();
            rectTransform = GetComponent<RectTransform>();
        }

    }
}