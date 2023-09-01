using System;
using System.Collections.Generic;
using AnimationPro.RunTime;
using Core.Utility.UI.Component;
using Core.Utility.UI.Component.Cursor;
using Core.Utility.UI.Navigation;
using Core.Utility.User;
using UnityEngine;

namespace UI.Title.StageSelect
{
    public class StageSelectManager : AnimationBehaviour, IUIManager
    {
        public enum Nav
        {
            Stage1,
            Stage2,
            Stage3,
            Stage4,
            Stage5,
            Back
        }

        [SerializeField] private SimpleUnderBarController bar;

        [SerializeField] private CursorController<Nav> cursor;
        [SerializeField] private StageButton stage1;
        [SerializeField] private StageButton stage2;
        [SerializeField] private StageButton stage3;
        [SerializeField] private StageButton stage4;
        [SerializeField] private StageButton stage5;
        [SerializeField] private FadeInOutButton back;

        private Nav? current;

        public event Action OnBack;

        public event Action<Nav> OnStage;

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

        public void SetScores(Dictionary<StageData.Stage, uint> scores)
        {
            foreach (var keyValuePair in scores)
            {
                switch (keyValuePair.Key)
                {
                    case StageData.Stage.Stage1:
                        stage1.SetScore(keyValuePair.Value);
                        break;
                    case StageData.Stage.Stage2:
                        stage2.SetScore(keyValuePair.Value);
                        break;
                    case StageData.Stage.Stage3:
                        stage3.SetScore(keyValuePair.Value);
                        break;
                    case StageData.Stage.Stage4:
                        stage4.SetScore(keyValuePair.Value);
                        break;
                    case StageData.Stage.Stage5:
                        stage4.SetScore(keyValuePair.Value);
                        break;
                }
            }
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
                case Nav.Stage1:
                    OnStage?.Invoke(Nav.Stage1);
                    break;
                case Nav.Stage2:
                    OnStage?.Invoke(Nav.Stage2);
                    break;
                case Nav.Stage3:
                    OnStage?.Invoke(Nav.Stage3);
                    break;
                case Nav.Stage4:
                    OnStage?.Invoke(Nav.Stage4);
                    break;
                case Nav.Stage5:
                    OnStage?.Invoke(Nav.Stage5);
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