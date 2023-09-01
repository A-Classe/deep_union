﻿using System;
using AnimationPro.RunTime;
using Core.Utility.UI;
using Core.Utility.UI.Cursor;
using Core.Utility.UI.UnderBar;
using UnityEngine;

namespace UI.Title.Option2
{
    /// <summary>
    ///     TL02-2
    ///     オプション画面
    ///     - FULL SCREEN
    ///     - BRIGHTNESS
    /// </summary>
    internal class Option2Manager : AnimationBehaviour, IUIManager
    {
        public enum Nav
        {
            FullScreen,
            Brightness,
            Back
        }

        [SerializeField] private SimpleUnderBarController bar;

        [SerializeField] private CursorController<Nav> cursor;
        [SerializeField] private ToggleController fullScreen;
        [SerializeField] private SliderController bright;
        [SerializeField] private FadeInOutButton back;

        private Nav? current;

        public event Action OnBack;

        public event Action<float> OnBrightness;

        public event Action<bool> OnFullScreen;

        private void Start()
        {
            cursor.AddPoint(Nav.FullScreen, fullScreen.rectTransform);
            cursor.AddPoint(Nav.Brightness, bright.rectTransform);
            cursor.AddPoint(Nav.Back, back.rectTransform);
            current = Nav.FullScreen;
            bright.Setup(100f, 0f, 70f);
        }

        public void Initialized(ContentTransform content)
        {
            gameObject.SetActive(true);
            bar.AnimateIn();
            OnCancel();
            Animation(content);
            SetState(Nav.FullScreen);
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
                case Nav.FullScreen:
                    OnFullScreenChanged(!fullScreen.IsOn);
                    break;
                case Nav.Brightness:
                    break;
                case Nav.Back:
                    OnFullScreen?.Invoke(fullScreen.IsOn);
                    OnBrightness?.Invoke(bright.Value);
                    back.OnPlay(() => OnBack?.Invoke());
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void Select(Vector2 direction)
        {
            if (Math.Abs(direction.x) > Math.Abs(direction.y))
            {
                if (current == Nav.Brightness)
                    OnBrightnessValueChanged(direction.x);
                else if (current == Nav.FullScreen) OnFullScreenChanged(!fullScreen.IsOn);

                return;
            }

            if (!current.HasValue)
            {
                SetState(Nav.FullScreen);
                return;
            }

            Nav nextNav;

            switch (direction.y)
            {
                // 上向きの入力
                case > 0:
                    nextNav = current.Value == Nav.FullScreen ? Nav.Back : current.Value - 1;
                    break;
                // 下向きの入力
                case < 0:
                    nextNav = current.Value == Nav.Back ? Nav.FullScreen : current.Value + 1;
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

        private void OnBrightnessValueChanged(float val)
        {
            var value = bright.Value + val;
            bright.SetValue(value);
        }

        private void OnFullScreenChanged(bool isOn)
        {
            fullScreen.SetState(isOn);
        }
    }
}