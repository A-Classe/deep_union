using System;
using AnimationPro.RunTime;
using Core.Utility.UI.Component;
using Core.Utility.UI.Component.Cursor;
using Core.Utility.UI.Navigation;
using UnityEngine;

namespace UI.Title.Option.Option1
{
    internal class Option1Manager : UIManager
    {
        public enum Nav
        {
            Video,
            Audio,
            KeyConfig,
            Back
        }

        [SerializeField] private CursorController<Nav> cursor;
        [SerializeField] private FadeInOutButton video;
        [SerializeField] private FadeInOutButton audios;
        [SerializeField] private FadeInOutButton keyConfig;
        [SerializeField] private FadeInOutButton back;

        private Nav? current;

        private void Start()
        {
            cursor.AddPoint(Nav.Video, video.rectTransform);
            cursor.AddPoint(Nav.Audio, audios.rectTransform);
            cursor.AddPoint(Nav.KeyConfig, keyConfig.rectTransform);
            cursor.AddPoint(Nav.Back, back.rectTransform);
            current = Nav.Video;
        }

        public override void Initialized(ContentTransform content)
        {
            base.Initialized(content);
            //SetState(Nav.Video);
        }

        /// <summary>
        ///     戻るボタンが押されたときに反映する
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public override void Clicked()
        {
            if (!current.HasValue) return;
            switch (current.Value)
            {
                case Nav.Video:
                    video.OnPlay(() => OnClick?.Invoke(Nav.Video));
                    break;
                case Nav.Audio:
                    audios.OnPlay(() => OnClick?.Invoke(Nav.Audio));
                    break;
                case Nav.KeyConfig:
                    keyConfig.OnPlay(() => OnClick?.Invoke(Nav.KeyConfig));
                    break;
                case Nav.Back:
                    back.OnPlay(() => OnBack?.Invoke());
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override void Select(Vector2 direction)
        {
            if (!current.HasValue)
            {
                SetState(Nav.Video);
                return;
            }

            Nav nextNav;

            switch (direction.y)
            {
                // 上向きの入力
                case > 0:
                    if(current.Value == Nav.Video)
                    {
                        return;
                    }
                    nextNav = current.Value - 1;
                    break;
                // 下向きの入力
                case < 0:
                    if(current.Value == Nav.Back)
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

        public event Action OnBack;

        public event Action<Nav> OnClick;


        private void SetState(Nav setNav)
        {
            current = setNav;
            cursor.SetPoint(setNav);
        }
    }
}