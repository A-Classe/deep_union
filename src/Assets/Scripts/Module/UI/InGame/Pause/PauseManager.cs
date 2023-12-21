using System;
using AnimationPro.RunTime;
using Core.Utility.UI.Component;
using Core.Utility.UI.Navigation;
using UnityEngine;

namespace Module.UI.InGame.Pause
{
    public class PauseManager : UIManager
    {
        public enum Nav
        {
            Resume,
            Option,
            Restart,
            Exit
        }

        [SerializeField] private FadeInOutButton resume;
        [SerializeField] private FadeInOutButton option;
        [SerializeField] private FadeInOutButton restart;
        [SerializeField] private FadeInOutButton exit;
        [SerializeField] private PauseCursor cursor;

        private Nav? current;

        private void Start()
        {
            cursor.AddPoint(Nav.Resume, resume.rectTransform);
            cursor.AddPoint(Nav.Option, option.rectTransform);
            cursor.AddPoint(Nav.Restart, restart.rectTransform);
            cursor.AddPoint(Nav.Exit, exit.rectTransform);
            current = Nav.Resume;
        }

        public override void Initialized(ContentTransform content, bool isReset = false)
        {
            base.Initialized(content, isReset);
            if (isReset)
            {
                SetState(Nav.Resume);
            }
        }

        /// <summary>
        ///     戻るボタンが押されたときに反映する
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public override void Clicked()
        {
            if (!current.HasValue)
            {
                return;
            }

            switch (current.Value)
            {
                case Nav.Resume:
                    resume.OnPlay(() => OnClick?.Invoke(Nav.Resume));
                    break;
                case Nav.Option:
                    option.OnPlay(() => OnClick?.Invoke(Nav.Option));
                    break;
                case Nav.Restart:
                    restart.OnPlay(() => OnClick?.Invoke(Nav.Restart));
                    break;
                case Nav.Exit:
                    exit.OnPlay(() => OnClick?.Invoke(Nav.Exit));
                    break;
            }
        }

        public override void Select(Vector2 direction)
        {
            if (!current.HasValue)
            {
                SetState(Nav.Resume);
                return;
            }

            Nav nextNav;

            switch (direction.y)
            {
                // 上向きの入力
                case > 0:
                    if (current.Value == Nav.Resume)
                    {
                        return;
                    }

                    nextNav = current.Value - 1;
                    break;
                // 下向きの入力
                case < 0:
                    if (current.Value == Nav.Exit)
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

        public event Action<Nav> OnClick;


        private void SetState(Nav setNav)
        {
            current = setNav;
            cursor.SetPoint(setNav);
        }
    }
}