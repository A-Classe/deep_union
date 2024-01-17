using System;
using System.Collections.Generic;
using System.Linq;
using Module.Assignment.Component;
using Module.Task;
using VContainer;

namespace Module.Assignment.System
{
    /// <summary>
    /// 有効化されているエリアを管理するクラス
    /// </summary>
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

        private void ActivateArea(BaseTask baseTask)
        {
            var assignableArea = baseTask.GetComponentInChildren<AssignableArea>();
            bool found = activeAreas.Any(area => area == assignableArea);

            //見つかったら重複回避のため終了
            if (found)
            {
                return;
            }

            assignableArea.enabled = true;
            activeAreas.Add(assignableArea);
        }

        private void DeactivateArea(BaseTask baseTask)
        {
            var assignableArea = baseTask.GetComponentInChildren<AssignableArea>();
            int index = activeAreas.IndexOf(assignableArea);

            //存在しなかったら終了
            if (index == -1)
            {
                return;
            }

            assignableArea.enabled = false;
            activeAreas.RemoveAt(index);
        }
    }
}