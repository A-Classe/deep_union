using System;
using System.Threading;
using Core.Model.User;
using Core.User.Recorder;
using Cysharp.Threading.Tasks;
using GameMain.Presenter;
using Module.UI.InGame;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using Wanna.DebugEx;

namespace Module.Player.Controller
{
    public class PlayerStatusUpdater : IInitializable, IDisposable
    {
        private readonly GameParam gameParam;
        private readonly PlayerStatus playerStatus;
        private readonly PlayerStatusVisualizer statusVisualizer;
        private readonly InGameUIManager uiManager;

        private readonly CancellationTokenSource cTokenSource;
        private short maxHp;

        private readonly EventBroker eventBroker;

        [Inject]
        public PlayerStatusUpdater(
            PlayerStatus playerStatus,
            GameParam gameParam,
            PlayerStatusVisualizer statusVisualizer,
            InGameUIManager uiManager,
            EventBroker eventBroker
        )
        {
            this.playerStatus = playerStatus;
            this.gameParam = gameParam;
            this.statusVisualizer = statusVisualizer;
            this.uiManager = uiManager;
            this.eventBroker = eventBroker;

            cTokenSource = new CancellationTokenSource();
        }

        public void Dispose()
        {
            cTokenSource?.Dispose();
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

        private async UniTaskVoid DecreaseHpLoop(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(gameParam.DecereseHpSpeed),
                    cancellationToken: cancellationToken);
                playerStatus.RemoveHp(gameParam.DecereseHpAmount);
            }
        }

        private void OnCallHpZero()
        {
            eventBroker.SendEvent(new GameOver().Event());
            uiManager.SetGameOver();
        }
    }
}