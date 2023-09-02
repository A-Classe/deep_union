using System.Collections.Generic;
using System.Text;
using Core.Debug;
using UnityDebugSheet.Runtime.Core.Scripts;
using UnityDebugSheet.Runtime.Core.Scripts.DefaultImpl.Cells;
using UnityEngine;
using UnityEngine.UI;
using Wanna.DebugEx;

namespace Module.Task
{
    class TaskObserveModel
    {
        private readonly BaseTask baseTask;
        private readonly LabelCellModel labelCellModel;
        private readonly StringBuilder stringBuilder;
        private readonly string mwData;

        public TaskObserveModel(BaseTask baseTask, ButtonCellModel labelCellModel)
        {
            this.baseTask = baseTask;

            stringBuilder = new StringBuilder();
            mwData = $"MW: {baseTask.MonoWork}\n";
        }

        public void Update()
        {
            //stringBuilder.Append(mwData);
            //stringBuilder.Append($"State: {baseTask.State}\n");
            stringBuilder.Append($"Progress: {baseTask.Progress * 100f}%\n");
            //stringBuilder.Append($"WorkerCount: {baseTask.WorkerCount}\n");

            labelCellModel.CellTexts.SubText = stringBuilder.ToString();

            stringBuilder.Clear();
        }
    }

    public class TaskDebugSheet
    {
        private readonly DebugToolPage debugToolPage;
        private readonly int itemId;
        private readonly List<(BaseTask task, Button button)> taskObserveButtons;


        TaskDebugPage taskDebugPage;

        public TaskDebugSheet()
        {
            taskObserveButtons = new List<(BaseTask task, Button button)>();
            debugToolPage = DebugSheet.Instance.GetOrCreateInitialPage<DebugToolPage>();
            itemId = debugToolPage.AddPageLinkButton<TaskDebugPage>("Task",
                icon: Resources.Load<Sprite>(AssetKeys.Resources.Icon.Model),
                onLoad: val =>
                {
                    taskDebugPage = val.page;

                    foreach (BaseTask baseTask in TaskUtil.FindSceneTasks<BaseTask>())
                    {
                        int pageId = taskDebugPage.AddPageLinkButton<TaskStatsPage>(baseTask.name, onLoad: page =>
                        {
                            page.page.SetUp(baseTask);
                        });

                        Button button = taskDebugPage.GetCellIfExists(pageId).GetComponent<Button>();
                        taskObserveButtons.Add((baseTask, button));
                    }
                });
        }

        public void Update()
        {
            if (taskDebugPage == null)
            {
                taskObserveButtons.Clear();
            }

            foreach ((BaseTask task, Button button) observeData in taskObserveButtons)
            {
                ColorBlock colorBlock = observeData.button.colors;
                colorBlock.normalColor = StateToColor(observeData.task.State);
            }

            Color StateToColor(TaskState state)
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


        public void Dispose()
        {
            if (debugToolPage != null)
            {
                debugToolPage.RemoveItem(itemId);
            }
        }
    }
}