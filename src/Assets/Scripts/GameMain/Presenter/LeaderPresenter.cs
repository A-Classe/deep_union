using Module.Assignment.Component;
using Module.Player.Controller;
using Module.Player.State;
using VContainer;
using VContainer.Unity;

namespace GameMain.Presenter
{
    public class LeaderPresenter : IInitializable
    {
        private readonly PlayerController playerController;
        private readonly LeaderAssignableArea leaderAssignableArea;

        [Inject]
        public LeaderPresenter(PlayerController playerController, LeaderAssignableArea leaderAssignableArea)
        {
            this.playerController = playerController;
            this.leaderAssignableArea = leaderAssignableArea;
        }

        public void Initialize()
        {
            playerController.OnStateChanged += state =>
            {
                leaderAssignableArea.SetWorldMovingActive(state == PlayerState.Go);
            };
        }
    }
}