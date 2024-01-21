using System;
using System.Collections.Generic;
using AnimationPro.RunTime;
using Core.Scenes;
using Core.User;
using Core.Utility.UI.Component;
using Core.Utility.UI.Component.Cursor;
using Core.Utility.UI.Navigation;
using UnityEngine;

namespace Module.UI.Title.StageSelect
{
    public class StageSelectManager : UIManager
    {
        [SerializeField] private SimpleUnderBarController bar;

        [SerializeField] private CursorController<StageNavigation> cursor;
        [SerializeField] private StageButton stage1;
        [SerializeField] private FadeInOutButton stage1Ranking;
        [SerializeField] private StageButton stage2;
        [SerializeField] private FadeInOutButton stage2Ranking;
        [SerializeField] private StageButton stage3;
        [SerializeField] private FadeInOutButton stage3Ranking;
        [SerializeField] private StageButton tutorial;
        [SerializeField] private FadeInOutButton back;

        private StageNavigation? current;

        private void Start()
        {
            cursor.AddPoint(StageNavigation.Stage1, stage1.rectTransform);
            cursor.AddPoint(StageNavigation.Stage1Ranking, stage1Ranking.rectTransform);
            cursor.AddPoint(StageNavigation.Stage2, stage2.rectTransform);
            cursor.AddPoint(StageNavigation.Stage2Ranking, stage2Ranking.rectTransform);
            cursor.AddPoint(StageNavigation.Stage3, stage3.rectTransform);
            cursor.AddPoint(StageNavigation.Stage3Ranking, stage3Ranking.rectTransform);
            cursor.AddPoint(StageNavigation.Tutorial, tutorial.rectTransform);
            cursor.AddPoint(StageNavigation.Back, back.rectTransform);
            current = StageNavigation.Stage1;

            SetState(StageNavigation.Stage1);
        }

        public override void Initialized(ContentTransform content, bool isReset = false)
        {
            base.Initialized(content, isReset);
            bar.AnimateIn();

            if (isReset)
            {
                SetState(StageNavigation.Stage1);
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
                case StageNavigation.Stage1:
                    OnStage?.Invoke(StageNavigation.Stage1);
                    break;
                case StageNavigation.Tutorial:
                    OnStage?.Invoke(StageNavigation.Tutorial);
                    break;
                case StageNavigation.Stage1Ranking:
                    OnRanking?.Invoke(StageData.Stage.Stage1);
                    break;
                case StageNavigation.Stage2:
                    OnStage?.Invoke(StageNavigation.Stage2);
                    break;
                case StageNavigation.Stage2Ranking:
                    OnRanking?.Invoke(StageData.Stage.Stage2);
                    break;
                case StageNavigation.Stage3:
                    OnStage?.Invoke(StageNavigation.Stage3);
                    break;
                case StageNavigation.Stage3Ranking:
                    OnRanking?.Invoke(StageData.Stage.Stage3);
                    break;
                case StageNavigation.Back:
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
                SetState(StageNavigation.Stage1);
                return;
            }

            StageNavigation nextNav = current.Value.Move(direction.x, direction.y);

            SetState(nextNav);
        }

        public override void Finished(ContentTransform content, Action onFinished)
        {
            bar.AnimateOut(() =>
            {
                base.Finished(content, onFinished);
            });
        }

        public event Action OnBack;

        public event Action<StageNavigation> OnStage;

        public event Action<StageData.Stage> OnRanking;

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
                }
            }
        }


        private void SetState(StageNavigation setNav)
        {
            current = setNav;
            cursor.SetPoint(setNav);
        }
    }
}