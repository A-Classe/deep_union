using System;
using AnimationPro.RunTime;
using UnityEngine;

namespace Core.Utility.UI.Component
{
    public class FadeInOutButton: AnimationBehaviour
    {
        [NonSerialized] public RectTransform rectTransform;

        protected override void Awake()
        {
            base.Awake();
            rectTransform = GetComponent<RectTransform>();
        }

        public void OnPlay(Action onFinished)
        {
            //FadeOut(() => FadeIn(onFinished));
            FadeOut(onFinished);
        }

        private void FadeIn(Action onFinished)
        {
            Animation(
                this.FadeIn(Easings.Default(0.2f)),
                new AnimationListener()
                {
                    OnFinished = onFinished
                }
            );
        }
        
        private void FadeOut(Action onFinished)
        {
            Animation(
                this.FadeOut(Easings.Default(0.2f)),
                new AnimationListener()
                {
                    OnFinished = onFinished
                }
            );
        }

        /// <summary>
        /// とりあえずで公開しとく
        /// </summary>
        public void InAnimation(ContentTransform a, AnimationListener listener)
        {
            Animation(a, listener);
        }
        
    }
}