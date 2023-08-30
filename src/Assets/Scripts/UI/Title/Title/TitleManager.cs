using System;
using AnimationPro.RunTime;
using Core.Utility.UI;
using Core.Utility.UI.Cursor;
using UnityEngine;

namespace UI.Title.Title
{
    internal class TitleManager : AnimationBehaviour, IUIManager
    {
        public enum Nav
        {
            Start,
            Option,
            Credit,
            Quit
        }

        [SerializeField] private CursorController<Nav> cursor;
        [SerializeField] private FadeInOutButton start;
        [SerializeField] private FadeInOutButton option;
        [SerializeField] private FadeInOutButton credit;
        [SerializeField] private FadeInOutButton quit;

        private Nav? current;

        public event Action OnCredit;

        public event Action OnOption;

        public event Action OnQuit;


        public event Action OnPlay;

        private void Start()
        {
            cursor.AddPoint(Nav.Start, start.rectTransform);
            cursor.AddPoint(Nav.Option, option.rectTransform);
            cursor.AddPoint(Nav.Credit, credit.rectTransform);
            cursor.AddPoint(Nav.Quit, quit.rectTransform);
            current = Nav.Start;
        }

        public void Initialized(ContentTransform content)
        {
            gameObject.SetActive(true);
            OnCancel();
            Animation(content);
            SetState(Nav.Start);
        }

        public void Clicked()
        {
            if (!current.HasValue) return;
            switch (current.Value)
            {
                case Nav.Start:
                    start.OnPlay(() => OnPlay?.Invoke());
                    break;
                case Nav.Option:
                    option.OnPlay(() => OnOption?.Invoke());
                    break;
                case Nav.Credit:
                    credit.OnPlay(() => OnCredit?.Invoke());
                    break;
                case Nav.Quit:
                    quit.OnPlay(() => OnQuit?.Invoke());
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void Select(Vector2 direction)
        {
            if (!current.HasValue)
            {
                SetState(Nav.Start);
                return;
            }

            Nav nextNav;

            switch (direction.y)
            {
                // 上向きの入力
                case > 0:
                    nextNav = current.Value == Nav.Start ? Nav.Quit : current.Value - 1;
                    break;
                // 下向きの入力
                case < 0:
                    nextNav = current.Value == Nav.Quit ? Nav.Start : current.Value + 1;
                    break;
                default:
                    return; // Y軸の入力がない場合、何もしない
            }

            SetState(nextNav);
        }

        public void Finished(ContentTransform content, Action onFinished)
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


        private void SetState(Nav setNav)
        {
            current = setNav;
            cursor.SetPoint(setNav);
        }
    }
}