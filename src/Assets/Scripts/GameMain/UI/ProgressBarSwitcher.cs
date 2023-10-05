using System.Collections.Generic;
using Module.Task;
using UI.HUD;
using VContainer;
using VContainer.Unity;

namespace GameMain.UI
{
    /// <summary>
    /// 進捗バーの更新を行うクラス
    /// </summary>
    public class ProgressBarSwitcher : IStartable, ITickable
    {
        private readonly TaskProgressPool taskProgressPool;
        private readonly TaskActivator taskActivator;
        private readonly Queue<(BaseTask task, TaskProgressView view)> activeViews;

        [Inject]
        public ProgressBarSwitcher(TaskProgressPool taskProgressPool, TaskActivator taskActivator)
        {
            this.taskProgressPool = taskProgressPool;
            this.taskActivator = taskActivator;
            activeViews = new Queue<(BaseTask task, TaskProgressView view)>();
        }

        public void Start()
        {
            foreach (var task in taskActivator.GetActiveTasks())
            {
                OnTaskActivated(task);
            }

            //ゲーム開始時に画面内に存在するタスクの進捗バー表示
            taskActivator.OnTaskActivated += OnTaskActivated;
            taskActivator.OnTaskDeactivated += OnTaskDeactivated;
        }

        public void Tick()
        {
            foreach ((BaseTask task, TaskProgressView view) element in activeViews)
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

        void OnTaskActivated(BaseTask task)
        {
            var view = taskProgressPool.GetProgressView(task.transform);
            activeViews.Enqueue((task, view));
        }

        void OnTaskDeactivated(BaseTask task)
        {
            var element = activeViews.Dequeue();
            taskProgressPool.ReleaseProgressView(element.view);
        }
    }
}