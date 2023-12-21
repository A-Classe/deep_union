using System;
using AnimationPro.RunTime;
using UnityEngine;

namespace Core.Utility.UI.Navigation
{
    public abstract class UIManager : AnimationBehaviour
    {
        public virtual void Initialized(ContentTransform content, bool isReset = false)
        {
            gameObject.SetActive(true);
            OnCancel();
            Animation(content);
        }

        public virtual void Select(Vector2 direction) { }

        public virtual void Clicked() { }

        public virtual void Finished(ContentTransform content, Action onFinished)
        {
            OnCancel();
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
    }
}