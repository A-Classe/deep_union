using UI.InGame;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Module.Player.Controller
{
    public class PlayerStatusUpdater : IInitializable
    {
        private readonly PlayerStatus playerStatus;
        private readonly PlayerStatusVisualizer statusVisualizer;
        private readonly InGameUIManager uiManager;
        private short maxHp;

        [Inject]
        public PlayerStatusUpdater(
            PlayerStatus playerStatus, 
            PlayerStatusVisualizer statusVisualizer,
            InGameUIManager uiManager
        )
        {
            this.playerStatus = playerStatus;
            this.statusVisualizer = statusVisualizer;
            this.uiManager = uiManager;
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
            playerStatus.OnCallHpZero += OnCallHpZero;
        }

        private void OnCallHpZero()
        {
            uiManager.SetGameOver();
        }
    }
}