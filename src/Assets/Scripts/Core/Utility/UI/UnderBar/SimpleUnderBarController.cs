using System;
using AnimationPro.RunTime;
using UnityEngine;

namespace Core.Utility.UI.UnderBar
{
    public class SimpleUnderBarController: MonoBehaviour
    {
        [SerializeField] private AnimateObject bar;
        [SerializeField] private AnimateObject leftAnchor;
        [SerializeField] private AnimateObject rightAnchor;

        public void AnimateIn(Action onFinished = null)
        {
            var easing = Easings.QuartIn(1f);
            bar.Animation(
                bar.ScaleIn(new Vector2(2f, 1f), easing),
                new AnimationListener()
                {
                    OnStart = () =>
                    {
                        bar.rectTransform.localScale = new Vector3(0f, 1f, 1f);  
                    },
                    OnFinished = () =>
                    {
                        bar.rectTransform.localScale = new Vector3(1f, 1f, 1f);
                        onFinished?.Invoke();
                    }
                }
            );
            leftAnchor.Animation(
                leftAnchor.SlideTo(new Vector2(-bar.rectTransform.rect.size.x / 2f, 0f), easing),
                new AnimationListener()
                {
                    OnStart = () =>
                    {
                        leftAnchor.rectTransform.localPosition -= new Vector3(-bar.rectTransform.rect.size.x / 2f, 0f, 0f);
                    }
                }
                );
            rightAnchor.Animation(
                rightAnchor.SlideTo(new Vector2(bar.rectTransform.rect.size.x / 2f, 0f), easing),
                new AnimationListener()
                {
                    OnStart = () =>
                    {
                        rightAnchor.rectTransform.localPosition -= new Vector3(bar.rectTransform.rect.size.x / 2f, 0f, 0f);
                    }
                }
            );

            
        }

        public void AnimateOut(Action onFinished = null)
        {
            var easing = Easings.QuartOut(1f);
            bar.Animation(
                bar.ScaleIn(new Vector2(0f, 1f), easing),
                new AnimationListener()
                {
                    OnStart = () =>
                    {
                        bar.rectTransform.localScale = new Vector3(1f, 1f, 1f);
                    },
                    OnFinished = () =>
                    {
                        bar.rectTransform.localScale = new Vector3(0f, 1f, 1f);
                        onFinished?.Invoke();
                    }
                }
            );
            leftAnchor.Animation(
                leftAnchor.SlideTo(new Vector2(bar.rectTransform.rect.size.x / 2f, 0f), easing),
                new AnimationListener()
                {
                    OnFinished = () =>
                    {
                        leftAnchor.rectTransform.localPosition -= new Vector3(-bar.rectTransform.rect.size.x / 2f, 0f, 0f);
                    }
                }
            );
            rightAnchor.Animation(
                rightAnchor.SlideTo(new Vector2(-bar.rectTransform.rect.size.x / 2f, 0f), easing),
                new AnimationListener()
                {
                    OnFinished = () =>
                    {
                        rightAnchor.rectTransform.localPosition -= new Vector3(bar.rectTransform.rect.size.x / 2f, 0f, 0f);
                    }
                }
            );
        }
    }
}