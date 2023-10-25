using System;
using AnimationPro.RunTime;
using Core.Utility.UI.Component;
using Core.Utility.UI.Component.Cursor;
using Core.Utility.UI.Navigation;
using UnityEngine;

namespace UI.InGame.Screen.GameOver
{
    /// <summary>
    /// ゲーム中のGameOverUI
    /// - リトライ
    /// - ステージセレクト
    /// </summary>
    public class GameOverManager: UIManager
    {
    
        public enum Nav
        {
            Retry,
            StageSelect
        }

        private Navigation<Nav> navigation;
        [SerializeField] private CursorController<Nav> cursor;
        [SerializeField] private FadeInOutButton retry;
        [SerializeField] private FadeInOutButton stage;

        private Nav? current;

        private void Start()
        {
            cursor.AddPoint(Nav.Retry, retry.rectTransform);
            cursor.AddPoint(Nav.StageSelect, stage.rectTransform);
            current = Nav.Retry;

            SetState(Nav.Retry);
        }

        public override void Initialized(ContentTransform content, bool isReset)
        {
            base.Initialized(content, isReset);

            if (isReset) { SetState(Nav.Retry); }
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
                case Nav.Retry:
                    retry.OnPlay(() => OnRetry?.Invoke());
                    break;
                case Nav.StageSelect:
                    stage.OnPlay(() => OnSelect?.Invoke());
                    break;
            }
        }

        public override void Select(Vector2 direction)
        {
            if (!current.HasValue)
            {
                SetState(Nav.Retry);
                return;
            }

            Nav nextNav;

            switch (direction.y)
            {
                // 上向きの入力
                case > 0:
                    nextNav = current.Value == Nav.StageSelect ? Nav.Retry : Nav.StageSelect;
                    break;
                // 下向きの入力
                case < 0:
                    nextNav = current.Value == Nav.Retry ? Nav.StageSelect : Nav.Retry;
                    break;
                default:
                    return; // Y軸の入力がない場合、何もしない
            }

            SetState(nextNav);
        }

        public event Action OnSelect;

        public event Action OnRetry;
        


        private void SetState(Nav setNav)
        {
            current = setNav;
            cursor.SetPoint(setNav);
        }
    }
    
}