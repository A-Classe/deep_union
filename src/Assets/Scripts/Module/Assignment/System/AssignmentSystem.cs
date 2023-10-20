using System.Collections.Generic;
using Core.Input;
using Module.Assignment.Component;
using Module.Task;
using VContainer;
using VContainer.Unity;
using Wanna.DebugEx;
using NotImplementedException = System.NotImplementedException;

namespace Module.Assignment.System
{
    public class AssignmentSystem : ITickable, IStartable
    {
        private readonly WorkerAssigner workerAssigner;
        private readonly WorkerReleaser workerReleaser;
        private readonly TaskActivator taskActivator;
        private readonly InputEvent assignEvent;
        private readonly InputEvent releaseEvent;
        private readonly List<AssignableArea> activeAreas;
        private AssignState assignState = AssignState.Idle;

        [Inject]
        public AssignmentSystem(WorkerAssigner workerAssigner, WorkerReleaser workerReleaser, TaskActivator taskActivator)
        {
            this.workerAssigner = workerAssigner;
            this.workerReleaser = workerReleaser;
            this.taskActivator = taskActivator;
            activeAreas = new List<AssignableArea>();

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

            taskActivator.OnTaskCreated += SetActiveAreas;
        }

        private void SetActiveAreas()
        {
            //タスクのアサインエリアを登録
            foreach (BaseTask task in taskActivator.GetActiveTasks())
            {
                AssignableArea assignableArea = task.GetComponentInChildren<AssignableArea>();
                assignableArea.enabled = true;
                activeAreas.Add(assignableArea);

                task.OnCompleted += OnTaskCompleted;
            }

            taskActivator.OnTaskActivated += task =>
            {
                AssignableArea assignableArea = task.GetComponentInChildren<AssignableArea>();
                assignableArea.enabled = true;
                activeAreas.Add(assignableArea);

                task.OnCompleted += OnTaskCompleted;
            };

            taskActivator.OnTaskDeactivated += task =>
            {
                activeAreas[0].enabled = false;
                OnTaskCompleted(task);
                activeAreas.RemoveAt(0);
            };

            workerAssigner.SetActiveAreas(activeAreas);
            workerReleaser.SetActiveAreas(activeAreas);
        }

        public void Start()
        {
            taskActivator.Start();
        }

        private void OnTaskCompleted(BaseTask task)
        {
            workerReleaser.ReleaseAllWorkers(task.GetComponentInChildren<AssignableArea>());
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