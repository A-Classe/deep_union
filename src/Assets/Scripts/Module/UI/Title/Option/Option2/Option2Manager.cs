using System;
using AnimationPro.RunTime;
using Core.Utility.UI.Component;
using Core.Utility.UI.Component.Cursor;
using Core.Utility.UI.Navigation;
using UnityEngine;

namespace Module.UI.Title.Option.Option2
{
    /// <summary>
    ///     TL02-2
    ///     オプション画面
    ///     - FULL SCREEN
    ///     - BRIGHTNESS
    /// </summary>
    internal class Option2Manager : UIManager
    {
        public enum Nav
        {
            Brightness,
            Back
        }

        [SerializeField] private CursorController<Nav> cursor;
        [SerializeField] private SliderController bright;
        [SerializeField] private FadeInOutButton back;

        private Nav? current;

        private void Start()
        {
            cursor.AddPoint(Nav.Brightness, bright.rectTransform);
            cursor.AddPoint(Nav.Back, back.rectTransform);
            current = Nav.Brightness;

            SetState(Nav.Brightness);
        }

        public override void Initialized(ContentTransform content, bool isReset = false)
        {
            base.Initialized(content, isReset);
            if (isReset) SetState(Nav.Brightness);
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
                case Nav.Brightness:
                    break;
                case Nav.Back:
                    OnBrightness?.Invoke(bright.Value);
                    back.OnPlay(() => OnBack?.Invoke());
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override void Select(Vector2 direction)
        {
            if (Math.Abs(direction.x) > Math.Abs(direction.y))
            {
                if (current == Nav.Brightness)
                    OnBrightnessValueChanged(direction.x);

                return;
            }

            if (!current.HasValue)
            {
                SetState(Nav.Brightness);
                return;
            }

            Nav nextNav;

            switch (direction.y)
            {
                // 上向きの入力
                case > 0:
                    if (current.Value == Nav.Brightness) return;
                    nextNav = current.Value - 1;
                    break;
                // 下向きの入力
                case < 0:
                    if (current.Value == Nav.Back) return;
                    nextNav = current.Value + 1;
                    break;
                default:
                    return; // Y軸の入力がない場合、何もしない
            }

            SetState(nextNav);
        }

        public event Action OnBack;

        public event Action<float> OnBrightness;

        public void SetValues(int brightVal)
        {
            // initialize
            bright.Setup(100f, 0f, 70f);

            bright.SetValue(brightVal);
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
    }
}