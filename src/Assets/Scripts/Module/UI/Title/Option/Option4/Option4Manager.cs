using System;
using AnimationPro.RunTime;
using Core.Utility.UI.Navigation;
using UnityEngine;

namespace Module.UI.Title.Option.Option4
{
    public class Option4Manager : UIManager
    {
        public override void Clicked()
        {
            OnBack?.Invoke();
        }

        public override void Select(Vector2 direction) { }

        public override void Finished(ContentTransform content, Action onFinished)
        {
            Animation(
                content,
                new AnimationListener
                {
                    OnFinished = () =>
                    {
                        gameObject.SetActive(false);
                        onFinished?.Invoke();
                    }
                }
            );
        }

        public event Action OnBack;
    }
}