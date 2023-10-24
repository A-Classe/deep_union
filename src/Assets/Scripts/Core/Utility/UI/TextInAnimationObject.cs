using System;
using AnimationPro.RunTime;
using TMPro;
using UnityEngine;

namespace Core.Utility.UI
{
    public class TextInAnimationObject: AnimationBehaviour
    {
        [NonSerialized] public RectTransform rectTransform;

        [SerializeField] private TextMeshProUGUI text;

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