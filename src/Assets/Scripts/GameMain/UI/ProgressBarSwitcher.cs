using System.Collections.Generic;
using Module.Task;
using Module.UI.HUD;
using VContainer;
using VContainer.Unity;

namespace GameMain.UI
{
    /// <summary>
    ///     進捗バーの更新を行うクラス
    /// </summary>
    public class ProgressBarSwitcher : ITickable
    {
        private readonly Queue<(BaseTask task, TaskProgressView view)> activeViews;
        private readonly TaskActivator taskActivator;
        private readonly TaskProgressPool taskProgressPool;

        [Inject]
        public ProgressBarSwitcher(TaskProgressPool taskProgressPool, TaskActivator taskActivator)
        {
            this.taskProgressPool = taskProgressPool;
            this.taskActivator = taskActivator;
            activeViews = new Queue<(BaseTask task, TaskProgressView view)>();

            this.taskActivator.OnTaskCreated += Initialize;
        }

        public void Tick()
        {
            foreach (var element in activeViews)
            {
                if (!element.view.IsEnabled)
                    continue;

                //タスクが終了したら非表示にする
                if (element.task.State == TaskState.Completed)
                {
                    element.view.Disable();
                    continue;
                }

                element.view.ManagedUpdate();
                element.view.SetProgress(element.task.Progress);
            }
        }

        public void Initialize()
        {
            foreach (var task in taskActivator.GetActiveTasks()) OnTaskActivated(task);

            //ゲーム開始時に画面内に存在するタスクの進捗バー表示
            taskActivator.OnTaskActivated += OnTaskActivated;
            taskActivator.OnTaskDeactivated += OnTaskDeactivated;
        }

        private void OnTaskActivated(BaseTask task)
        {
            var view = taskProgressPool.GetProgressView(task.transform);
            activeViews.Enqueue((task, view));
        }

        private void OnTaskDeactivated(BaseTask task)
        {
            var element = activeViews.Dequeue();
            taskProgressPool.ReleaseProgressView(element.view);
        }
    }
}