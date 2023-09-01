using System;
using AnimationPro.RunTime;
using TMPro;
using UnityEngine;

namespace Core.Utility.UI.Component
{
    public class StageButton: AnimationBehaviour
    {
        [SerializeField] private TextMeshProUGUI text;

        public void SetScore(uint score)
        {
            text.text = score.ToString();
        }
        
        [NonSerialized] public RectTransform rectTransform;

        protected override void Awake()
        {
            base.Awake();
            rectTransform = GetComponent<RectTransform>();
        }
    }
}