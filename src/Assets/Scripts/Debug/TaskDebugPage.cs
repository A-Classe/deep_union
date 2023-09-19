using System.Collections.Generic;
using System.Threading.Tasks;
using Module.Task;
using UnityDebugSheet.Runtime.Core.Scripts;
using UnityEngine;
using UnityEngine.UI;

namespace Debug
{
    public sealed class TaskDebugPage : DefaultDebugPageBase
    {
        protected override string Title => "Task";
        private List<TaskObserveModel> taskObserveButtons;

        public override Task Initialize()
        {
            taskObserveButtons = new List<TaskObserveModel>();

            BaseTask[] baseTasks = TaskUtil.FindSceneTasks<BaseTask>();

            foreach (var baseTask in baseTasks)
            {
                TaskPageLinkButtonCellModel buttonCell = new TaskPageLinkButtonCellModel(false);
                buttonCell.CellTexts.Text = baseTask.name;
                buttonCell.PageType = typeof(TaskStatsPage);
                buttonCell.ShowArrow = true;

                buttonCell.OnLoad += page =>
                {
                    TaskStatsPage debugPage = page.page as TaskStatsPage;
                    if (debugPage != null)
                    {
                        debugPage.SetUp(baseTask);
                    }
                };

                int id = AddItem("TaskPageLinkButtonCell", buttonCell);
                taskObserveButtons.Add(new TaskObserveModel(id, baseTask, buttonCell));
            }

            return base.Initialize();
        }

        private void Update()
        {
            foreach (TaskObserveModel observeData in taskObserveButtons)
            {
                observeData.UpdatedData();
            }

            RefreshData();
        }

        readonly struct TaskObserveModel
        {
            private readonly int id;
            private readonly BaseTask task;
            private readonly TaskPageLinkButtonCellModel model;

            public TaskObserveModel(int id, BaseTask task, TaskPageLinkButtonCellModel model)
            {
                this.id = id;
                this.task = task;
                this.model = model;
            }

            public int UpdatedData()
            {
                ColorBlock colorBlock = new ColorBlock();
                colorBlock.normalColor = StateToColor(task.State);
                colorBlock.highlightedColor = StateToColor(task.State);
                colorBlock.pressedColor = StateToColor(task.State) * 0.8f;
                colorBlock.colorMultiplier = 1f;
                colorBlock.fadeDuration = 0.1f;
                model.ColorBlock = colorBlock;

                return id;
            }

            private Color StateToColor(TaskState state)
            {
                switch (state)
                {
                    case TaskState.Idle:
                        return Color.red;
                    case TaskState.InProgress:
                        return Color.yellow;
                    case TaskState.Completed:
                        return Color.green;
                }

                return Color.white;
            }
        }
    }
}