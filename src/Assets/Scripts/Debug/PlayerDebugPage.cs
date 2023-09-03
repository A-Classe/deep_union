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
        private LeadPointConnector leadPointConnector;

        private LabelObserver<PlayerController> speedObserver;
        private LabelObserver<PlayerController> stateObserver;
        private LabelObserver<LeadPointConnector> workerCountObserver;
        private LabelObserver<ResourceContainer> resourceObserver;

        public void SetUp(ResourceContainer resourceContainer, LeadPointConnector leadPointConnector)
        {
            this.resourceContainer = resourceContainer;
            this.leadPointConnector = leadPointConnector;
            playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();

            speedObserver = LabelObserver<PlayerController>.Create(this, controller => $"Speed: {controller.Speed}");
            stateObserver = LabelObserver<PlayerController>.Create(this, controller => $"State: {controller.GetState()}");
            workerCountObserver = LabelObserver<LeadPointConnector>.Create(this, leader => $"WorkerCount: {leader.WorkerCount}");
            resourceObserver = LabelObserver<ResourceContainer>.Create(this, container => $"ResourceCount: {container.ResourceCount}");
        }

        private void Update()
        {
            speedObserver.Update(playerController);
            stateObserver.Update(playerController);
            workerCountObserver.Update(leadPointConnector);
            resourceObserver.Update(resourceContainer);

            RefreshData();
        }
    }
}