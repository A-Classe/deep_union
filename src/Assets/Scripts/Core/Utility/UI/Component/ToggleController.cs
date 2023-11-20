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

        [SerializeField] private TextMeshProUGUI text;

        [NonSerialized] public RectTransform rectTransform;

        public bool IsOn { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            rectTransform = GetComponent<RectTransform>();
        }

        public void SetState(bool isOn)
        {
            IsOn = isOn;
            text.text = isOn ? OnText : OffText;
        }
    }
}