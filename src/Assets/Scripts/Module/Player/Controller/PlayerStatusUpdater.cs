using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using GameMain.Presenter;
using UI.InGame;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Module.Player.Controller
{
    public class PlayerStatusUpdater : IInitializable, IDisposable
    {
        private readonly PlayerStatus playerStatus;
        private readonly GameParam gameParam;
        private readonly PlayerStatusVisualizer statusVisualizer;
        private readonly InGameUIManager uiManager;

        private CancellationTokenSource cTokenSource;
        private short maxHp;

        [Inject]
        public PlayerStatusUpdater(
            PlayerStatus playerStatus,
            GameParam gameParam,
            PlayerStatusVisualizer statusVisualizer,
            InGameUIManager uiManager
        )
        {
            this.playerStatus = playerStatus;
            this.gameParam = gameParam;
            this.statusVisualizer = statusVisualizer;
            this.uiManager = uiManager;

            cTokenSource = new CancellationTokenSource();
        }

        public void Initialize()
        {
            maxHp = playerStatus.MaxHp;
            uiManager.SetHp(maxHp, maxHp);

            playerStatus.OnHpChanged += hp =>
            {
                uiManager.SetHp(hp);
                statusVisualizer.SetHpRate(1 - Mathf.InverseLerp(0f, maxHp, hp));
            };

            DecreaseHpLoop(cTokenSource.Token).Forget();
            playerStatus.OnCallHpZero += OnCallHpZero;
        }

        async UniTaskVoid DecreaseHpLoop(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(gameParam.DecereseHpSpeed), cancellationToken: cancellationToken);
                playerStatus.RemoveHp(gameParam.DecereseHpAmount);
            }
        }
        
        private void OnCallHpZero()
        {
            uiManager.SetGameOver();
        }

        public void Dispose()
        {
            cTokenSource?.Dispose();
        }
    }
}