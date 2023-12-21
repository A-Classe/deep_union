using Core.Model.User;
using Core.User.Recorder;
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
        private readonly EventBroker eventBroker;

        [Inject]
        public LeaderPresenter(
            PlayerController playerController,
            LeaderAssignableArea leaderAssignableArea,
            EventBroker eventBroker
        )
        {
            this.playerController = playerController;
            this.leaderAssignableArea = leaderAssignableArea;
            this.eventBroker = eventBroker;
        }

        public void Start()
        {
            playerController.OnStateChanged += state =>
            {
                leaderAssignableArea.SetWorldMovingActive(state != PlayerState.Pause);
            };
            leaderAssignableArea.OnChangedWorkers += type =>
            {
                switch (type)
                {
                    case AssignableArea.WorkerEventType.Create:
                        eventBroker.SendEvent(new AddWorker().Event());
                        break;
                    case AssignableArea.WorkerEventType.Destroy:
                        eventBroker.SendEvent(new DelWorker().Event());
                        break;
                }
            };
        }
    }
}