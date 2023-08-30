using System;
using AnimationPro.RunTime;
using Core.Utility.UI;
using Core.Utility.UI.Cursor;
using Core.Utility.UI.UnderBar;
using UnityEngine;

namespace UI.Title.Option3
{
    public class Option3Manager : AnimationBehaviour, IUIManager
    {
        public enum Nav
        {
            Master,
            Music,
            Effect,
            Back
        }

        [SerializeField] private SimpleUnderBarController bar;

        [SerializeField] private CursorController<Nav> cursor;
        [SerializeField] private SliderController master;
        [SerializeField] private SliderController music;
        [SerializeField] private SliderController effect;
        [SerializeField] private FadeInOutButton back;

        private Nav? current;

        public Action onBack;

        public Action<Nav, float> onVolumeChanged;

        private void Start()
        {
            cursor.AddPoint(Nav.Master, master.rectTransform);
            cursor.AddPoint(Nav.Music, music.rectTransform);
            cursor.AddPoint(Nav.Effect, effect.rectTransform);
            cursor.AddPoint(Nav.Back, back.rectTransform);
            current = Nav.Master;
            master.Setup(100f, 0f, 70f);
            music.Setup(100f, 0f, 70f);
            effect.Setup(100f, 0f, 70f);
        }

        public void Initialized(ContentTransform content)
        {
            gameObject.SetActive(true);
            bar.AnimateIn();
            OnCancel();
            Animation(content);
            SetState(Nav.Master);
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
                case Nav.Back:
                    onVolumeChanged?.Invoke(Nav.Master, master.Value);
                    onVolumeChanged?.Invoke(Nav.Music, music.Value);
                    onVolumeChanged?.Invoke(Nav.Effect, effect.Value);
                    onBack?.Invoke();
                    break;
                default:
                    return;
            }
        }

        public void Select(Vector2 direction)
        {
            if (!current.HasValue)
            {
                SetState(Nav.Master);
                return;
            }

            if (Math.Abs(direction.x) > Math.Abs(direction.y))
            {
                switch (current)
                {
                    case Nav.Master:
                        master.SetValue(master.Value + direction.x);
                        onVolumeChanged?.Invoke(Nav.Master, master.Value);
                        break;
                    case Nav.Music:
                        music.SetValue(music.Value + direction.x);
                        onVolumeChanged?.Invoke(Nav.Music, music.Value);
                        break;
                    case Nav.Effect:
                        effect.SetValue(effect.Value + direction.x);
                        onVolumeChanged?.Invoke(Nav.Effect, effect.Value);
                        break;
                    case Nav.Back:
                        back.OnPlay(() => onBack?.Invoke());
                        break;
                }

                return;
            }

            Nav nextNav;

            switch (direction.y)
            {
                // 上向きの入力
                case > 0:
                    nextNav = current.Value == Nav.Master ? Nav.Back : current.Value - 1;
                    break;
                // 下向きの入力
                case < 0:
                    nextNav = current.Value == Nav.Back ? Nav.Master : current.Value + 1;
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