using System;
using AnimationPro.RunTime;
using TMPro;
using UnityEngine;

namespace Core.Utility.UI.Component
{
    public class ToggleController : AnimationBehaviour
    {
        private const string OnText = "ON";

        private const string OffText = "OFF";
        
        public bool IsOn { get; private set; }

        [SerializeField] private TextMeshProUGUI text;
        
        public void SetState(bool isOn)
        {
            IsOn = isOn;
            text.text = isOn ? OnText : OffText;
        }
        
        [NonSerialized] public RectTransform rectTransform;

        protected override void Awake()
        {
            base.Awake();
            rectTransform = GetComponent<RectTransform>();
        }

        
    }
}