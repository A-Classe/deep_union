using System;
using AnimationPro.RunTime;
using Core.Utility.UI.Component;
using Core.Utility.UI.Component.Cursor;
using Core.Utility.UI.Navigation;
using UnityEngine;

namespace Module.UI.Title.Quit
{
    public class QuitManager : UIManager
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

        private void Start()
        {
            cursor.AddPoint(Nav.Yes, yes.rectTransform);
            cursor.AddPoint(Nav.No, no.rectTransform);
            current = null;

            SetState(Nav.No);
        }

        public override void Initialized(ContentTransform content, bool isReset = false)
        {
            base.Initialized(content, isReset);
            bar.AnimateIn();

            if (isReset)
            {
                SetState(Nav.No);
            }
        }

        public override void Clicked()
        {
            if (!current.HasValue)
            {
                return;
            }

            switch (current.Value)
            {
                case Nav.Yes:
                    yes.OnPlay(() => OnClick?.Invoke(true));
                    break;
                case Nav.No:
                    no.OnPlay(() => OnClick?.Invoke(false));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override void Select(Vector2 direction)
        {
            if (!current.HasValue)
            {
                SetState(Nav.Yes);
                return;
            }

            Nav nextNav;

            switch (direction.y)
            {
                case > 0:
                    if (current.Value == Nav.Yes)
                    {
                        return;
                    }

                    nextNav = Nav.Yes;
                    break;

                case < 0:
                    if (current.Value == Nav.No)
                    {
                        return;
                    }

                    nextNav = Nav.No;
                    break;

                default:
                    return;
            }

            SetState(nextNav);
        }

        public override void Finished(ContentTransform content, Action onFinished)
        {
            bar.AnimateOut(() =>
            {
                base.Finished(content, onFinished);
            });
        }

        public event Action<bool> OnClick;


        private void SetState(Nav setNav)
        {
            current = setNav;
            cursor.SetPoint(setNav);
        }
    }
}