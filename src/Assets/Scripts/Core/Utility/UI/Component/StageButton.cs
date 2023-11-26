using System;
using AnimationPro.RunTime;
using TMPro;
using UnityEngine;

namespace Core.Utility.UI.Component
{
    public class StageButton : AnimationBehaviour
    {
        [SerializeField] private TextMeshProUGUI text;

        [NonSerialized] public RectTransform rectTransform;

        protected override void Awake()
        {
            base.Awake();
            rectTransform = GetComponent<RectTransform>();
        }

        public void SetScore(uint score)
        {
            if (text == null) return;
            text.text = score.ToString();
        }
    }
}