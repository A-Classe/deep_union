using System;
using System.Collections.Generic;
using Core.Debug;
using UnityDebugSheet.Runtime.Core.Scripts;
using UnityDebugSheet.Runtime.Core.Scripts.DefaultImpl.Cells;

namespace Module.Task
{
    readonly struct CellData
    {
        public readonly int CellId;
        public readonly LabelCellModel CellModel;
        private readonly Func<BaseTask, string> getText;

        public CellData(int cellId, LabelCellModel cellModel, Func<BaseTask, string> getText)
        {
            CellId = cellId;
            CellModel = cellModel;
            this.getText = getText;
        }

        public string GetText(BaseTask baseTask)
        {
            return getText(baseTask);
        }
    }

    public class TaskStatsPage : DefaultDebugPageBase
    {
        private static readonly string PageTitle = "Task Stats";
        protected override string Title => PageTitle;

        private List<CellData> observableCells;
        private BaseTask baseTask;

        public void SetUp(BaseTask baseTask)
        {
            this.baseTask = baseTask;

            AddLabel($"Name: {baseTask.name}");
            AddLabel($"MonoWork: {baseTask.MonoWork}");

            LabelCellModel stateCell = new LabelCellModel(false);
            LabelCellModel progressCell = new LabelCellModel(false);
            LabelCellModel workerCell = new LabelCellModel(false);

            observableCells = new List<CellData>
            {
                new CellData(AddLabel(stateCell), stateCell, task => $"State: {task.State}"),
                new CellData(AddLabel(progressCell), progressCell, task => $"Progress: {task.Progress * 100f}%"),
                new CellData(AddLabel(workerCell), workerCell, task => $"WorkerCount: {task.WorkerCount}"),
            };
        }

        public void Update()
        {
            foreach (CellData cell in observableCells)
            {
                cell.CellModel.CellTexts.Text = cell.GetText(baseTask);
                RefreshDataAt(cell.CellId);
            }
        }
    }
}