using Module.Assignment;
using Module.Player.Controller;
using Module.Task;
using Module.Working.Controller;
using UnityDebugSheet.Runtime.Core.Scripts;
using UnityEngine;

namespace Debug
{
    public class PlayerDebugPage : DefaultDebugPageBase
    {
        protected override string Title => "Player";
        private PlayerController playerController;
        private ResourceContainer resourceContainer;
        private LeaderAssignEvent leaderAssignEvent;

        private LabelObserver<PlayerController> speedObserver;
        private LabelObserver<PlayerController> stateObserver;
        private LabelObserver<LeaderAssignEvent> workerCountObserver;
        private LabelObserver<ResourceContainer> resourceObserver;

        public void SetUp(ResourceContainer resourceContainer, LeaderAssignEvent leaderAssignEvent)
        {
            this.resourceContainer = resourceContainer;
            playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();

            speedObserver = LabelObserver<PlayerController>.Create(this, controller => $"Speed: {controller.Speed}");
            stateObserver = LabelObserver<PlayerController>.Create(this, controller => $"State: {controller.GetState()}");
            workerCountObserver = LabelObserver<LeaderAssignEvent>.Create(this, leader => $"WorkerCount: {leaderAssignEvent.WorkerCount}");
            resourceObserver = LabelObserver<ResourceContainer>.Create(this, container => $"ResourceCount: {container.ResourceCount}");
        }

        private void Update()
        {
            speedObserver.Update(playerController);
            stateObserver.Update(playerController);
            workerCountObserver.Update(leaderAssignEvent);
            resourceObserver.Update(resourceContainer);

            RefreshData();
        }
    }
}