using System;
using AnimationPro.RunTime;
using Core.Utility.UI.Component;
using Core.Utility.UI.Component.Cursor;
using Core.Utility.UI.Navigation;
using UnityEngine;

namespace UI.Title.Quit
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
        }

        public override void Initialized(ContentTransform content)
        {
            base.Initialized(content);
            bar.AnimateIn();
            SetState(Nav.Yes);
        }

        public override void Clicked()
        {
            if (!current.HasValue) return;
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

            if (direction.y != 0)
            {
                var nextNav = current == Nav.No ? Nav.Yes : Nav.No;
                SetState(nextNav);
            }
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