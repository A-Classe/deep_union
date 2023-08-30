using System;
using AnimationPro.RunTime;
using Core.Utility.UI;
using Core.Utility.UI.Cursor;
using Core.Utility.UI.UnderBar;
using UnityEngine;

namespace UI.Title.Quit
{
    internal class QuitManager : AnimationBehaviour, IUIManager
    {
        public enum Nav
        {
            Yes,
            No
        }

        [SerializeField] private SimpleUnderBarController bar;

        [SerializeField] private CursorController<Nav> cursor;
        [SerializeField] private FadeInOutButton yes;
        [SerializeField] private FadeInOutButton no;

        private Nav? current;

        public Action<bool> onClick;

        private void Start()
        {
            cursor.AddPoint(Nav.Yes, yes.rectTransform);
            cursor.AddPoint(Nav.No, no.rectTransform);
            current = null;
        }

        public void Initialized(ContentTransform content)
        {
            gameObject.SetActive(true);
            bar.AnimateIn();
            OnCancel();
            Animation(content);
            SetState(Nav.Yes);
        }

        public void Clicked()
        {
            if (!current.HasValue) return;
            switch (current.Value)
            {
                case Nav.Yes:
                    yes.OnPlay(() => onClick?.Invoke(true));
                    break;
                case Nav.No:
                    no.OnPlay(() => onClick?.Invoke(false));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void Select(Vector2 direction)
        {
            if (!current.HasValue)
            {
                SetState(Nav.Yes);
                return;
            }

            if (direction.y != 0)
            {
                var nextNav = current == Nav.No ? Nav.Yes : Nav.No;
                SetState(nextNav);
            }
        }

        public void Finished(ContentTransform content, Action onFinished)
        {
            bar.AnimateOut(() =>
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
            });
        }

        public AnimationBehaviour GetContext()
        {
            return this;
        }


        private void SetState(Nav setNav)
        {
            current = setNav;
            cursor.SetPoint(setNav);
        }
    }
}