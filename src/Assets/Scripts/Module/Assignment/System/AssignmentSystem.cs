using System.Collections.Generic;
using Core.Input;
using Core.User.Recorder;
using Module.Assignment.Component;
using Module.Task;
using VContainer;
using VContainer.Unity;
using Wanna.DebugEx;

namespace Module.Assignment.System
{
    public class AssignmentSystem : ITickable, IStartable
    {
        private readonly InputEvent assignEvent;
        private readonly InputEvent releaseEvent;
        private readonly TaskActivator taskActivator;
        private readonly WorkerAssigner workerAssigner;
        private readonly WorkerReleaser workerReleaser;
        private readonly EventBroker eventBroker;
        private AssignState assignState = AssignState.Idle;

        [Inject]
        public AssignmentSystem(
            WorkerAssigner workerAssigner,
            WorkerReleaser workerReleaser,
            TaskActivator taskActivator,
            ActiveAreaCollector activeAreaCollector,
            EventBroker eventBroker
        )
        {
            this.workerAssigner = workerAssigner;
            this.workerReleaser = workerReleaser;
            this.taskActivator = taskActivator;

            assignEvent = InputActionProvider.Instance.CreateEvent(ActionGuid.InGame.Assign);
            releaseEvent = InputActionProvider.Instance.CreateEvent(ActionGuid.InGame.Release);
            
            assignEvent.Started += _ =>
            {
                assignState = AssignState.Assign;
            };

            assignEvent.Canceled += _ =>
            {
                assignState = AssignState.Idle;
            };

            releaseEvent.Started += _ =>
            {
                assignState = AssignState.Release;
            };

            releaseEvent.Canceled += _ =>
            {
                assignState = AssignState.Idle;
            };
        }

        public void Start()
        {
            taskActivator.Start();
        }

        public void Tick()
        {
            switch (assignState)
            {
                case AssignState.Assign:
                    workerAssigner.Update();
                    break;
                case AssignState.Release:
                    workerReleaser.Update();
                    break;
            }
        }
    }
}