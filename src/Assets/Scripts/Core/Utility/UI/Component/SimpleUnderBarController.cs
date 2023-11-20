using System;
using AnimationPro.RunTime;
using UnityEngine;

namespace Core.Utility.UI.Component
{
    public class SimpleUnderBarController : MonoBehaviour
    {
        [SerializeField] private AnimateObject bar;
        [SerializeField] private AnimateObject leftAnchor;
        [SerializeField] private AnimateObject rightAnchor;
        private Vector3 fPosLeft;
        private Vector3 fPosRight;

        private Vector3 iPosLeft;

        private Vector3 iPosRight;

        private bool isInit;

        private void Start()
        {
            if (isInit)
            {
                leftAnchor.rectTransform.localPosition = fPosLeft;
                rightAnchor.rectTransform.localPosition = fPosRight;
            }

            ;
            iPosLeft = leftAnchor.rectTransform.localPosition;
            fPosLeft = iPosLeft - new Vector3(-bar.rectTransform.rect.size.x / 2f, 0f, 0f);
            iPosRight = rightAnchor.rectTransform.localPosition;
            fPosRight = iPosRight - new Vector3(bar.rectTransform.rect.size.x / 2f, 0f, 0f);
            isInit = true;

            leftAnchor.rectTransform.localPosition = fPosLeft;
            rightAnchor.rectTransform.localPosition = fPosRight;
        }

        public void AnimateIn(Action onFinished = null)
        {
            var easing = Easings.QuartOut(1f);
            bar.OnCancel();
            leftAnchor.OnCancel();
            rightAnchor.OnCancel();
            bar.Animation(
                bar.ScaleIn(new Vector2(2f, 1f), easing),
                new AnimationListener
                {
                    OnStart = () => { bar.rectTransform.localScale = new Vector3(0f, 1f, 1f); },
                    OnFinished = () =>
                    {
                        bar.rectTransform.localScale = new Vector3(1f, 1f, 1f);
                        onFinished?.Invoke();
                    }
                }
            );
            leftAnchor.Animation(
                leftAnchor.SlideTo(new Vector2(-bar.rectTransform.rect.size.x / 2f, 0f), easing),
                new AnimationListener
                {
                    OnFinished = () => { leftAnchor.rectTransform.localPosition = iPosLeft; }
                }
            );
            rightAnchor.Animation(
                rightAnchor.SlideTo(new Vector2(bar.rectTransform.rect.size.x / 2f, 0f), easing),
                new AnimationListener
                {
                    OnFinished = () => { rightAnchor.rectTransform.localPosition = iPosRight; }
                }
            );
        }

        public void AnimateOut(Action onFinished = null)
        {
            leftAnchor.rectTransform.localPosition = iPosLeft;
            rightAnchor.rectTransform.localPosition = iPosRight;
            var easing = Easings.QuartOut(1f);
            bar.OnCancel();
            leftAnchor.OnCancel();
            rightAnchor.OnCancel();
            bar.Animation(
                bar.ScaleIn(new Vector2(0f, 1f), easing),
                new AnimationListener
                {
                    OnStart = () => { bar.rectTransform.localScale = new Vector3(1f, 1f, 1f); },
                    OnFinished = () =>
                    {
                        bar.rectTransform.localScale = new Vector3(0f, 1f, 1f);
                        onFinished?.Invoke();
                    }
                }
            );
            leftAnchor.Animation(
                leftAnchor.SlideTo(new Vector2(bar.rectTransform.rect.size.x / 2f, 0f), easing),
                new AnimationListener
                {
                    OnFinished = () => { leftAnchor.rectTransform.localPosition = fPosLeft; }
                }
            );
            rightAnchor.Animation(
                rightAnchor.SlideTo(new Vector2(-bar.rectTransform.rect.size.x / 2f, 0f), easing),
                new AnimationListener
                {
                    OnFinished = () => { rightAnchor.rectTransform.localPosition = fPosRight; }
                }
            );
        }
    }
}