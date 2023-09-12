using System.Collections.Generic;
using Module.Task;
using UnityDebugSheet.Runtime.Core.Scripts;

namespace Debug
{
    public class TaskStatsPage : DefaultDebugPageBase
    {
        private static readonly string PageTitle = "Task Stats";
        protected override string Title => PageTitle;

        private List<LabelObserver<BaseTask>> observableCells;
        private BaseTask baseTask;

        public void SetUp(BaseTask baseTask)
        {
            this.baseTask = baseTask;

            AddLabel($"Name: {baseTask.name}");
            AddLabel($"MonoWork: {baseTask.MonoWork}");

            observableCells = new List<LabelObserver<BaseTask>>
            {
                LabelObserver<BaseTask>.Create(this, task => $"State: {task.State}"),
                LabelObserver<BaseTask>.Create(this, task => $"Progress: {task.Progress * 100f}%"),
                LabelObserver<BaseTask>.Create(this, task => $"WorkerCount: {task.WorkerCount}"),
            };
        }

        public void Update()
        {
            foreach (LabelObserver<BaseTask> cell in observableCells)
            {
                cell.Update(baseTask);
                RefreshDataAt(cell.CellId);
            }
        }
    }
}