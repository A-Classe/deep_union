using System;
using AnimationPro.RunTime;
using Core.Model.Scene;
using Core.Utility.UI;
using Core.Utility.UI.Component;
using Core.Utility.UI.Component.Cursor;
using Core.Utility.UI.Navigation;
using UnityEngine;

namespace UI.Result
{
    /// <summary>
    /// リザルト画面のセレクター
    /// </summary>
    public class ResultManager: UIManager
    {
        public enum Nav
        {
            Next,
            Retry,
            Select
        }

        [SerializeField] private CursorController<Nav> cursor;
        [SerializeField] private FadeInOutButton next;
        [SerializeField] private FadeInOutButton retry;
        [SerializeField] private FadeInOutButton select;

        [SerializeField] private TextInAnimationObject worker;
        [SerializeField] private TextInAnimationObject hp;
        [SerializeField] private TextInAnimationObject resource;
        [SerializeField] private TextInAnimationObject score;
        [SerializeField] private TextInAnimationObject result;

        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip gameClear;

        private Nav? current;

        private void Start()
        {
            cursor.AddPoint(Nav.Next, next.rectTransform);
            cursor.AddPoint(Nav.Retry, retry.rectTransform);
            cursor.AddPoint(Nav.Select, select.rectTransform);
            current = Nav.Next;
            
            SetState(Nav.Next);

            audioSource.PlayOneShot(gameClear);
        }

        public override void Initialized(ContentTransform content, bool isReset)
        {
            base.Initialized(content, isReset);
            
            if (isReset) { SetState(Nav.Next); }
        }

        public override void Clicked()
        {
            if (!current.HasValue) return;
            switch (current.Value)
            {
                case Nav.Next:
                    next.OnPlay(() => OnNext?.Invoke());
                    break;
                case Nav.Retry:
                    retry.OnPlay(() => OnRetry?.Invoke());
                    break;
                case Nav.Select:
                    select.OnPlay(() => OnSelect?.Invoke());
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override void Select(Vector2 direction)
        {
            if (!current.HasValue)
            {
                SetState(Nav.Next);
                return;
            }

            Nav nextNav;

            switch (direction.y)
            {
                // 上向きの入力
                case > 0:
                    nextNav = current.Value == Nav.Next ? Nav.Next : current.Value - 1;
                    break;
                // 下向きの入力
                case < 0:
                    nextNav = current.Value == Nav.Select ? Nav.Select : current.Value + 1;
                    break;
                default:
                    return; // Y軸の入力がない場合、何もしない
            }

            SetState(nextNav);
        }
        

        public event Action OnNext;

        public event Action OnRetry;

        public event Action OnSelect;
        


        private void SetState(Nav setNav)
        {
            current = setNav;
            cursor.SetPoint(setNav);
        }

        public void SetScore(GameResult result, Action onFinished)
        {
            worker.SetText(result.WorkerCount.ToString());
            hp.SetText(result.Hp.ToString());
            resource.SetText(result.Resource.ToString());
            this.result.SetText(result.GetScore().ToString());
            StartAnimation(onFinished);
        }

        public void StartAnimation(Action onFinished)
        {
            worker.Animation(worker.FadeIn(Easings.Default(0.5f, 0.4f)));
            hp.Animation(hp.FadeIn(Easings.Default(0.5f, 1f)));
            resource.Animation(resource.FadeIn(Easings.Default(0.5f, 1.6f)));
            score.Animation(score.FadeIn(Easings.Default(0.5f, 2.2f)));
            result.Animation(resource.FadeIn(Easings.Default(0.5f, 2.2f)),
                new AnimationListener
                {
                    OnFinished = onFinished
                }
            );
        }
    }
}