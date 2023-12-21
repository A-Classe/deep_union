using System;
using System.Collections;
using AnimationPro.RunTime;
using TMPro;
using UnityEngine;

namespace Core.Utility.UI.Component
{
    public class PopupText : SwitchableAnimationBehaviour
    {
        [SerializeField] private TextMeshProUGUI textObject;

        public override ContentTransform EnterTransform { get; set; }
        public override ContentTransform ExitTransform { get; set; }

        private readonly float rate = 1.3f;
        private readonly float delaySec = 0.2f;

        private void Start()
        {
            if (textObject == null)
            {
                throw new NotImplementedException();
            }

            EnterTransform = this.ScaleOut(rate, Easings.Default(delaySec));
            ExitTransform = this.ScaleIn(2 - rate, Easings.Default(delaySec));
        }

        public void SetTextIsUp(string text)
        {
            textObject.text = text;
            State = true;
            StartCoroutine(AnimateFinished());
        }

        public void SetTextIsDown(string text)
        {
            textObject.text = text;
            State = true;
            StartCoroutine(AnimateFinished());
        }

        private IEnumerator AnimateFinished()
        {
            yield return new WaitForSeconds(0.6f);
            State = false;
        }
    }
}