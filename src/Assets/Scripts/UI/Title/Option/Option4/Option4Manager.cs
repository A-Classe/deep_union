using System;
using AnimationPro.RunTime;
using Core.Utility.UI.Navigation;
using UnityEngine;

namespace UI.Title.Option.Option4
{
    public class Option4Manager : AnimationBehaviour, IUIManager
    {
        public void Initialized(ContentTransform content)
        {
            gameObject.SetActive(true);
            OnCancel();
            Animation(content);
        }

        public void Clicked()
        {
            OnBack?.Invoke();
        }

        public void Select(Vector2 direction)
        {
        }

        public void Finished(ContentTransform content, Action onFinished)
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

        public AnimationBehaviour GetContext()
        {
            return this;
        }

        public event Action OnBack;
    }
}