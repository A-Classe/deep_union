using System.Collections.Generic;
using Module.Assignment.Component;
using Module.Task;
using VContainer;

namespace Module.Assignment.System
{
    public class ActiveAreaCollector
    {
        private readonly WorkerAssigner workerAssigner;
        private readonly WorkerReleaser workerReleaser;
        private readonly TaskActivator taskActivator;
        private readonly List<AssignableArea> activeAreas;

        [Inject]
        public ActiveAreaCollector(
            WorkerAssigner workerAssigner,
            WorkerReleaser workerReleaser,
            TaskActivator taskActivator
        )
        {
            this.workerAssigner = workerAssigner;
            this.workerReleaser = workerReleaser;
            this.taskActivator = taskActivator;

            activeAreas = new List<AssignableArea>();

            taskActivator.OnTaskCreated += SetActiveAreas;
        }

        private void SetActiveAreas()
        {
            //タスクのアサインエリアを登録
            foreach (var task in taskActivator.GetActiveTasks())
            {
                ActiveArea(task);
            }

            taskActivator.OnTaskActivated += ActiveArea;

            taskActivator.OnTaskDeactivated += task =>
            {
                activeAreas[0].enabled = false;
                activeAreas.RemoveAt(0);
            };

            workerAssigner.SetActiveAreas(activeAreas);
            workerReleaser.SetActiveAreas(activeAreas);
        }

        public void ActiveArea(BaseTask baseTask)
        {
            var assignableArea = baseTask.GetComponentInChildren<AssignableArea>();
            assignableArea.enabled = true;
            activeAreas.Add(assignableArea);
        }
    }
}