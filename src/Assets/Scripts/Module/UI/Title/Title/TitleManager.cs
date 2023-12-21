using System;
using Core.Utility.UI.Component;
using Core.Utility.UI.Component.Cursor;
using Core.Utility.UI.Navigation;
using UnityEngine;

namespace Module.UI.Title.Title
{
    public class TitleManager : UIManager
    {
        public enum Nav
        {
            Start,
            Option,
            Credit,
            Stats,
            Quit
        }

        [SerializeField] private CursorController<Nav> cursor;
        [SerializeField] private FadeInOutButton start;
        [SerializeField] private FadeInOutButton option;
        [SerializeField] private FadeInOutButton credit;
        [SerializeField] private FadeInOutButton stats;
        [SerializeField] private FadeInOutButton quit;

        private Nav? current;

        private void Start()
        {
            cursor.AddPoint(Nav.Start, start.rectTransform);
            cursor.AddPoint(Nav.Option, option.rectTransform);
            cursor.AddPoint(Nav.Credit, credit.rectTransform);
            cursor.AddPoint(Nav.Stats, stats.rectTransform);
            cursor.AddPoint(Nav.Quit, quit.rectTransform);
            current = Nav.Start;

            SetState(Nav.Start);
        }

        public override void Clicked()
        {
            if (!current.HasValue)
            {
                return;
            }

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
                case Nav.Stats:
                    stats.OnPlay(() => OnStats?.Invoke());
                    break;
                case Nav.Quit:
                    quit.OnPlay(() => OnQuit?.Invoke());
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override void Select(Vector2 direction)
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
                    if (current.Value == Nav.Start)
                    {
                        return;
                    }

                    nextNav = current.Value - 1;
                    break;
                // 下向きの入力
                case < 0:
                    if (current.Value == Nav.Quit)
                    {
                        return;
                    }

                    nextNav = current.Value + 1;
                    break;
                default:
                    return; // Y軸の入力がない場合、何もしない
            }

            SetState(nextNav);
        }


        public event Action OnCredit;

        public event Action OnOption;

        public event Action OnQuit;

        public event Action OnStats;

        public event Action OnPlay;


        private void SetState(Nav setNav)
        {
            current = setNav;
            cursor.SetPoint(setNav);
        }
    }
}