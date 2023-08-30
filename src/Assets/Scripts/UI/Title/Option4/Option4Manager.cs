using System;
using AnimationPro.RunTime;
using Core.Utility.UI.UnderBar;
using UnityEngine;

namespace UI.Title.Option4
{
    public class Option4Manager: AnimationBehaviour, IUIManager
    {
        [SerializeField] private SimpleUnderBarController bar;


        public void Initialized(ContentTransform content)
        {     
            gameObject.SetActive(true);
            bar.AnimateIn();
            OnCancel();
            Animation(content);
            bar.AnimateIn();
        }
        public void Clicked()
        { }

        public void Select(Vector2 direction)
        {
        }

        public void Finished(ContentTransform content, Action onFinished)
        {
            bar.AnimateOut(() =>
                {
                    Animation(
                        content,
                        new AnimationListener()
                        {
                            OnFinished = () =>
                            {
                                gameObject.SetActive(false);
                                onFinished?.Invoke();
                            }
                        }
                    );    
                }
            );
        }
        
        public AnimationBehaviour GetContext() => this;
    }
}