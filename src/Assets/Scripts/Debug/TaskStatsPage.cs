using System.Collections.Generic;
using Module.Task;
using UnityDebugSheet.Runtime.Core.Scripts;

namespace Debug
{
    public class TaskStatsPage : DefaultDebugPageBase
    {
        private static readonly string PageTitle = "Task Stats";
        private BaseTask baseTask;

        private List<LabelObserver<BaseTask>> observableCells;
        protected override string Title => PageTitle;

        public void Update()
        {
            foreach (var cell in observableCells)
            {
                cell.Update(baseTask);
                RefreshDataAt(cell.CellId);
            }
        }

        public void SetUp(BaseTask baseTask)
        {
            this.baseTask = baseTask;

            AddLabel($"Name: {baseTask.name}");
            AddLabel($"MonoWork: {baseTask.MonoWork}");

            observableCells = new List<LabelObserver<BaseTask>>
            {
                LabelObserver<BaseTask>.Create(this, task => $"State: {task.State}"),
                LabelObserver<BaseTask>.Create(this, task => $"Progress: {task.Progress * 100f}%"),
                LabelObserver<BaseTask>.Create(this, task => $"WorkerCount: {task.WorkerCount}")
            };
        }
    }
}