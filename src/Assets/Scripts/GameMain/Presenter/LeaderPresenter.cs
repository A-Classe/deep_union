using Module.Assignment.Component;
using Module.Player.Controller;
using Module.Player.State;
using VContainer;
using VContainer.Unity;

namespace GameMain.Presenter
{
    public class LeaderPresenter : IStartable
    {
        private readonly LeaderAssignableArea leaderAssignableArea;
        private readonly PlayerController playerController;

        [Inject]
        public LeaderPresenter(PlayerController playerController, LeaderAssignableArea leaderAssignableArea)
        {
            this.playerController = playerController;
            this.leaderAssignableArea = leaderAssignableArea;
        }

        public void Start()
        {
            playerController.OnStateChanged += state =>
            {
                leaderAssignableArea.SetWorldMovingActive(state != PlayerState.Pause);
            };
        }
    }
}