using System.Collections.Generic;
using Module.Task;
using Module.UI.HUD;
using VContainer;
using VContainer.Unity;
using Wanna.DebugEx;

namespace Module.Extension.UI
{
    /// <summary>
    ///     進捗バーの更新を行うクラス
    /// </summary>
    public class ProgressBarSwitcher : ITickable
    {
        private readonly Dictionary<BaseTask, TaskProgressView> activeViews;
        private readonly TaskActivator taskActivator;
        private readonly TaskProgressPool taskProgressPool;

        [Inject]
        public ProgressBarSwitcher(TaskProgressPool taskProgressPool, TaskActivator taskActivator)
        {
            this.taskProgressPool = taskProgressPool;
            this.taskActivator = taskActivator;
            activeViews = new Dictionary<BaseTask, TaskProgressView>(64);

            this.taskActivator.OnTaskCreated += Initialize;
        }

        public void Tick()
        {
            foreach ((BaseTask task, TaskProgressView view) in activeViews)
            {
                if (!view.IsEnabled)
                    continue;

                //タスクが終了したら非表示にする
                if (task.State == TaskState.Completed)
                {
                    view.Disable();
                    continue;
                }

                view.ManagedUpdate();
                view.SetProgress(task.Progress);
            }
        }

        private void Initialize()
        {
            foreach (var task in taskActivator.GetActiveTasks())
            {
                OnTaskActivated(task);
            }

            //ゲーム開始時に画面内に存在するタスクの進捗バー表示
            taskActivator.OnTaskActivated += OnTaskActivated;
            taskActivator.OnTaskDeactivated += OnTaskDeactivated;
        }

        private void OnTaskActivated(BaseTask task)
        {
            var view = taskProgressPool.GetProgressView(task.transform);
            activeViews.Add(task, view);
        }

        private void OnTaskDeactivated(BaseTask task)
        {
            taskProgressPool.ReleaseProgressView(activeViews[task]);
            activeViews.Remove(task);
        }
    }
}