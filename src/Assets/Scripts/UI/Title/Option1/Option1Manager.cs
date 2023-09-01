﻿using System;
using AnimationPro.RunTime;
using Core.Utility.UI;
using Core.Utility.UI.Cursor;
using Core.Utility.UI.UnderBar;
using UnityEngine;

namespace UI.Title.Option1
{
    internal class Option1Manager : AnimationBehaviour, IUIManager
    {
        public enum Nav
        {
            Video,
            Audio,
            KeyConfig,
            Back
        }

        [SerializeField] private SimpleUnderBarController bar;

        [SerializeField] private CursorController<Nav> cursor;
        [SerializeField] private FadeInOutButton video;
        [SerializeField] private FadeInOutButton audios;
        [SerializeField] private FadeInOutButton keyConfig;
        [SerializeField] private FadeInOutButton back;

        private Nav? current;

        public event Action OnBack;

        public event Action<Nav> OnClick;

        private void Start()
        {
            cursor.AddPoint(Nav.Video, video.rectTransform);
            cursor.AddPoint(Nav.Audio, audios.rectTransform);
            cursor.AddPoint(Nav.KeyConfig, keyConfig.rectTransform);
            cursor.AddPoint(Nav.Back, back.rectTransform);
            current = Nav.Video;
        }

        public void Initialized(ContentTransform content)
        {
            gameObject.SetActive(true);
            bar.AnimateIn();
            OnCancel();
            Animation(content);
            SetState(Nav.Video);
        }

        /// <summary>
        ///     戻るボタンが押されたときに反映する
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public void Clicked()
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