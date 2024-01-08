using Core.Model.User;
using Core.User.Recorder;
using Module.Assignment.Component;
using Module.UI.InGame;
using Module.Working;
using VContainer;
using VContainer.Unity;

namespace GameMain.Container
{
    public class LeaderStatusPresenter : IInitializable
    {
        private readonly LeaderAssignableArea leaderAssignableArea;
        private readonly InGameUIManager uiManager;
        private readonly EventBroker eventBroker;

        [Inject]
        public LeaderStatusPresenter(
            WorkerAgent workerAgent,
            InGameUIManager uiManager,
            EventBroker eventBroker
        )
        {
            this.uiManager = uiManager;
            this.eventBroker = eventBroker;

            workerAgent.OnWorkerCountUpdated += OnChangedWorkers;
        }

        private void OnChangedWorkers(uint count)
        {
            //ワーカーの数が0になったらゲームオーバー
            if (count == 0)
            {
                eventBroker.SendEvent(new GameOver().Event());
                uiManager.SetGameOver();
            }
        }

        public void Initialize() { }
    }
}