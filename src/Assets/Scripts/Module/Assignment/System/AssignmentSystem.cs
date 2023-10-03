using System.Collections.Generic;
using Core.Input;
using Module.Assignment.Component;
using Module.Task;
using VContainer;
using VContainer.Unity;
using Wanna.DebugEx;

namespace Module.Assignment.System
{
    public class AssignmentSystem : ITickable
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

            SetActiveAreas();
        }

        private void SetActiveAreas()
        {
            //タスクのアサインエリアを登録
            foreach (BaseTask task in taskActivator.GetActiveTasks())
            {
                activeAreas.Add(task.GetComponentInChildren<AssignableArea>());

                task.OnCompleted += OnTaskCompleted;
            }

            taskActivator.OnTaskActivated += task =>
            {
                activeAreas.Add(task.GetComponentInChildren<AssignableArea>());

                task.OnCompleted += OnTaskCompleted;
            };

            taskActivator.OnTaskDeactivated += task =>
            {
                OnTaskCompleted(task);
                activeAreas.RemoveAt(0);
            };

            workerAssigner.SetActiveAreas(activeAreas);
            workerReleaser.SetActiveAreas(activeAreas);
        }

        private void OnTaskCompleted(BaseTask task)
        {
            workerReleaser.ReleaseAllWorkers(task.GetComponentInChildren<AssignableArea>());
            task.OnCompleted -= OnTaskCompleted;
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