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
        private readonly LeadPointConnector leadPointConnector;

        [Inject]
        public LeaderPresenter(PlayerController playerController, LeadPointConnector leadPointConnector)
        {
            this.playerController = playerController;
            this.leadPointConnector = leadPointConnector;
        }

        public void Initialize()
        {
            playerController.OnStateChanged += state =>
            {
                leadPointConnector.SetWorldMovingActive(state == PlayerState.Go);
            };
        }
    }
}