using System;
using AnimationPro.RunTime;
using UnityEngine;

namespace Core.Utility.UI
{
    public class ToggleController : AnimationBehaviour
    {
        public bool IsOn { get; private set; }
        
        public void SetState(bool isOn)
        {
            IsOn = isOn;
        }
        
        [NonSerialized] public RectTransform rectTransform;

        protected override void Awake()
        {
            base.Awake();
            rectTransform = GetComponent<RectTransform>();
        }

        
    }
}