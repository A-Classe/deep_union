using Module.Assignment;
using Module.Player.Controller;
using Module.Player.State;
using Module.Working.Controller;
using VContainer;
using VContainer.Unity;

namespace GameMain.Presenter
{
    public class LeaderPresenter : IInitializable
    {
        private readonly PlayerController playerController;
        private readonly LeaderAssignEvent leaderAssignEvent;

        [Inject]
        public LeaderPresenter(PlayerController playerController, LeaderAssignEvent leaderAssignEvent)
        {
            this.playerController = playerController;
            this.leaderAssignEvent = leaderAssignEvent;
        }

        public void Initialize()
        {
            playerController.OnStateChanged += state =>
            {
                leaderAssignEvent.SetWorldMovingActive(state == PlayerState.Go);
            };
        }
    }
}