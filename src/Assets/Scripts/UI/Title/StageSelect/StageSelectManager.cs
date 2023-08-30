using System;
using AnimationPro.RunTime;
using Core.Utility.UI;
using Core.Utility.UI.Cursor;
using Core.Utility.UI.UnderBar;
using UnityEngine;

namespace UI.Title.StageSelect
{
    public class StageSelectManager: AnimationBehaviour, IUIManager
    {
        [SerializeField] private SimpleUnderBarController bar;
        
        public Action<Nav> onStage;

        public Action onBack;
        
        public enum Nav
        {
            Stage1,
            Stage2,
            Stage3,
            Stage4,
            Stage5,
            Back
        }

        [SerializeField] private CursorController<Nav> cursor;
        [SerializeField] private FadeInOutButton stage1;
        [SerializeField] private FadeInOutButton stage2;
        [SerializeField] private FadeInOutButton stage3;
        [SerializeField] private FadeInOutButton stage4;
        [SerializeField] private FadeInOutButton stage5;
        [SerializeField] private FadeInOutButton back;

        private Nav? current;


        private void SetState(Nav setNav)
        {
            current = setNav;
            cursor.SetPoint(setNav);
        }

        private void Start()
        {
            cursor.AddPoint(Nav.Stage1, stage1.rectTransform);
            cursor.AddPoint(Nav.Stage2, stage2.rectTransform);
            cursor.AddPoint(Nav.Stage3, stage3.rectTransform);
            cursor.AddPoint(Nav.Stage4, stage4.rectTransform);
            cursor.AddPoint(Nav.Stage5, stage5.rectTransform);
            cursor.AddPoint(Nav.Back, back.rectTransform);
            current = Nav.Stage1;
        }

        public void Initialized(ContentTransform content)
        {     
            gameObject.SetActive(true);
            bar.AnimateIn();
            OnCancel();
            Animation(content);
            SetState(Nav.Stage1);
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
                case Nav.Stage1:
                    onStage?.Invoke(Nav.Stage1);
                    break;
                case Nav.Stage2:
                    onStage?.Invoke(Nav.Stage2);
                    break;
                case Nav.Stage3:
                    onStage?.Invoke(Nav.Stage3);
                    break;
                case Nav.Stage4:
                    onStage?.Invoke(Nav.Stage4);
                    break;
                case Nav.Stage5:
                    onStage?.Invoke(Nav.Stage5);
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
                SetState(Nav.Stage1);
                return;
            }

            Nav nextNav;
            
            switch (direction.y)
            {
                // 上向きの入力
                case > 0:
                    nextNav = current.Value == Nav.Stage1 ? Nav.Back : current.Value - 1;
                    break;
                // 下向きの入力
                case < 0:
                    nextNav = current.Value == Nav.Back ? Nav.Stage1 : current.Value + 1;
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
                    new AnimationListener()
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
        
        public AnimationBehaviour GetContext() => this;
    }
}