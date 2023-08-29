using System;
using Core.Utility.UI;
using Core.Utility.UI.Cursor;
using UnityEngine;

namespace UI.Title.Option2
{
    /// <summary>
    /// TL02-2
    /// オプション画面
    /// - FULL SCREEN
    /// - BRIGHTNESS
    /// </summary>
    internal class Option2Manager : MonoBehaviour, IUIManager
    {
        public Action<bool> onFullScreen;

        public Action<float> onBrightness;

        public Action onBack;
        
        public enum Nav
        {
            FullScreen,
            Brightness,
            Back
        }

        [SerializeField] private CursorController<Nav> cursor;
        [SerializeField] private ToggleController fullScreen;
        [SerializeField] private SliderController bright;
        [SerializeField] private RectTransform back;

        private Nav? current;

        private void Start()
        {
            cursor.AddPoint(Nav.FullScreen, fullScreen.rectTransform);
            cursor.AddPoint(Nav.Brightness, bright.rectTransform);
            cursor.AddPoint(Nav.Back, back);
            current = Nav.FullScreen;
            bright.Setup(100f, 0f, 70f);
        }


        private void SetState(Nav setNav)
        {
            current = setNav;
            cursor.SetPoint(setNav);
        }

        public void Initialized()
        {     
            gameObject.SetActive(true);
            SetState(Nav.FullScreen);
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
                case Nav.FullScreen:
                    OnFullScreenChanged(!fullScreen.IsOn);
                    break;
                case Nav.Brightness:
                    break;
                case Nav.Back:
                    onFullScreen(fullScreen.IsOn);
                    onBrightness(bright.Value);
                    onBack?.Invoke();
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
                {
                    OnBrightnessValueChanged(direction.x);
                } else if (current == Nav.FullScreen)
                {
                    OnFullScreenChanged(!fullScreen.IsOn);
                }
               
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

        private void OnBrightnessValueChanged(float val)
        {
            float value = bright.Value + val;
            bright.SetValue(value);
        }

        private void OnFullScreenChanged(bool isOn)
        {
            fullScreen.SetState(isOn);
        }

        public void Finished()
        {
            gameObject.SetActive(false);
        }
    }
}