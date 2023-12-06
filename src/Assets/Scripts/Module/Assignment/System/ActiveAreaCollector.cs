using System;
using System.Collections.Generic;
using System.Linq;
using Module.Assignment.Component;
using Module.Task;
using VContainer;
using Wanna.DebugEx;

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

            taskActivator.OnTaskInitialized += SetActiveAreas;
        }

        private void SetActiveAreas(ReadOnlyMemory<BaseTask> initializedTasks)
        {
            //タスクのアサインエリアを登録
            foreach (var task in initializedTasks.Span)
            {
                ActivateArea(task);
            }

            taskActivator.OnTaskActivated += ActivateArea;
            taskActivator.OnTaskDeactivated += DeactivateArea;

            workerAssigner.SetActiveAreas(activeAreas);
            workerReleaser.SetActiveAreas(activeAreas);
        }

        public void ActivateArea(BaseTask baseTask)
        {
            var assignableArea = baseTask.GetComponentInChildren<AssignableArea>();
            assignableArea.enabled = true;

            bool found = activeAreas.Any(area => area == assignableArea);
            DebugEx.Assert(!found, "タスクを重複して追加することは出来ません");

            if (!found)
            {
                activeAreas.Add(assignableArea);
            }
        }

        public void DeactivateArea(BaseTask baseTask)
        {
            var assignableArea = baseTask.GetComponentInChildren<AssignableArea>();
            assignableArea.enabled = false;

            int index = activeAreas.IndexOf(assignableArea);
            DebugEx.Assert(index != -1, "存在しないタスクを削除することは出来ません");

            if (index != -1)
            {
                activeAreas.RemoveAt(index);
            }
        }
    }
}