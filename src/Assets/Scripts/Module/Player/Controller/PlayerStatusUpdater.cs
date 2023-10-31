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
            uiManager.SetHP(maxHp, maxHp);

            playerStatus.OnHpChanged += hp =>
            {
                uiManager.SetHP(hp);
                statusVisualizer.SetHpRate(1 - Mathf.InverseLerp(0f, maxHp, hp));
            };

            DecreaseHpLoop(cTokenSource.Token).Forget();
        }

        async UniTaskVoid DecreaseHpLoop(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(gameParam.DecereseHpSpeed), cancellationToken: cancellationToken);
                playerStatus.RemoveHp(gameParam.DecereseHpAmount);
            }
        }

        public void Dispose()
        {
            cTokenSource?.Dispose();
        }
    }
}