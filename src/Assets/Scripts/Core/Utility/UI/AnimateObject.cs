using System;
using AnimationPro.RunTime;
using UnityEngine;

namespace Core.Utility.UI
{
    public class AnimateObject : AnimationBehaviour
    {
        [NonSerialized] public RectTransform rectTransform;

        protected override void Awake()
        {
            base.Awake();
            rectTransform = GetComponent<RectTransform>();
        }
    }
}