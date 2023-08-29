using System;
using Core.Utility.UI;
using Core.Utility.UI.Cursor;
using UnityEngine;

namespace UI.Title.Option1
{
    internal class Option1Manager : MonoBehaviour, IUIManager
    {
        public Action<Nav> onClick;

        public Action onBack;
        
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


        private void SetState(Nav setNav)
        {
            current = setNav;
            cursor.SetPoint(setNav);
        }

        public void Initialized()
        {     
            gameObject.SetActive(true);
            SetState(Nav.Video);
        }

        /// <summary>
        /// 戻るボタンが押されたときに反映する
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public void Clicked()
        {
            if (!current.HasValue) return;
            switch (current.Value)
            {
                case Nav.Video:
                    video.OnPlay(() => onClick?.Invoke(Nav.Video));
                    break;
                case Nav.Audio:
                    audios.OnPlay(() => onClick?.Invoke(Nav.Audio));
                    break;
                case Nav.KeyConfig:
                    keyConfig.OnPlay(() => onClick?.Invoke(Nav.KeyConfig));
                    break;
                case Nav.Back:
                    back.OnPlay(() => onBack?.Invoke());
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void Select(Vector2 direction)
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
                    nextNav = current.Value == Nav.Video ? Nav.Back : current.Value - 1;
                    break;
                // 下向きの入力
                case < 0:
                    nextNav = current.Value == Nav.Back ? Nav.Video : current.Value + 1;
                    break;
                default:
                    return; // Y軸の入力がない場合、何もしない
            }

            SetState(nextNav);
        }

        public void Finished()
        {
            gameObject.SetActive(false);
        }
    }
}