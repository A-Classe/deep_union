using Module.Assignment;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Module.Player.Controller
{
    public class PlayerStatusUpdater : IInitializable
    {
        private readonly PlayerStatus playerStatus;
        private readonly PlayerStatusVisualizer statusVisualizer;
        private short maxHp;

        [Inject]
        public PlayerStatusUpdater(PlayerStatus playerStatus, PlayerStatusVisualizer statusVisualizer)
        {
            this.playerStatus = playerStatus;
            this.statusVisualizer = statusVisualizer;
        }

        public void Initialize()
        {
            maxHp = playerStatus.MaxHp;

            playerStatus.OnHpChanged += hp =>
            {
                statusVisualizer.SetHpRate(1 - Mathf.InverseLerp(0f, maxHp, hp));
            };
        }
    }
}