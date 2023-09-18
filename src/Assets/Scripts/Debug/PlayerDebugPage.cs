using Module.Assignment;
using Module.Assignment.Component;
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
        private LeaderAssignableArea leaderAssignableArea;

        private LabelObserver<PlayerController> speedObserver;
        private LabelObserver<PlayerController> stateObserver;
        private LabelObserver<LeaderAssignableArea> workerCountObserver;
        private LabelObserver<ResourceContainer> resourceObserver;

        public void SetUp(ResourceContainer resourceContainer, LeaderAssignableArea leaderAssignableArea)
        {
            this.resourceContainer = resourceContainer;
            playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();

            speedObserver = LabelObserver<PlayerController>.Create(this, controller => $"Speed: {controller.Speed}");
            stateObserver = LabelObserver<PlayerController>.Create(this, controller => $"State: {controller.GetState()}");
            workerCountObserver = LabelObserver<LeaderAssignableArea>.Create(this, leader => $"WorkerCount: {leaderAssignableArea.WorkerCount}");
            resourceObserver = LabelObserver<ResourceContainer>.Create(this, container => $"ResourceCount: {container.ResourceCount}");
        }

        private void Update()
        {
            speedObserver.Update(playerController);
            stateObserver.Update(playerController);
            workerCountObserver.Update(leaderAssignableArea);
            resourceObserver.Update(resourceContainer);

            RefreshData();
        }
    }
}