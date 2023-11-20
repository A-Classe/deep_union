using System;
using AnimationPro.RunTime;
using TMPro;
using UnityEngine;

namespace Core.Utility.UI
{
    public class TextInAnimationObject : AnimationBehaviour
    {
        [SerializeField] private TextMeshProUGUI text;
        [NonSerialized] public RectTransform rectTransform;

        protected override void Awake()
        {
            base.Awake();
            rectTransform = GetComponent<RectTransform>();
        }

        public void SetText(string text)
        {
            this.text.text = text;
        }
    }
}