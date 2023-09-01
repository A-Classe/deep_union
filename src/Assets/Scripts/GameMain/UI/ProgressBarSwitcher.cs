using System;
using System.Collections.Generic;
using System.Linq;
using Module.Task;
using UI.HUD;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using Wanna.DebugEx;

namespace GameMain.UI
{
    /// <summary>
    /// 進捗バーの更新を行うクラス
    /// </summary>
    public class ProgressBarSwitcher : IStartable, ITickable
    {
        private readonly Camera mainCamera;
        private readonly TaskProgressPool taskProgressPool;
        private readonly BaseTask[] tasks;
        private readonly Queue<(BaseTask task, TaskProgressView view)> activeViews;

        private int head;
        private int tail;

        [Inject]
        public ProgressBarSwitcher(TaskProgressPool taskProgressPool)
        {
            mainCamera = Camera.main;
            activeViews = new Queue<(BaseTask task, TaskProgressView view)>();
            this.taskProgressPool = taskProgressPool;
            tasks = SortTaskOrder(TaskUtil.FindSceneTasks<BaseTask>());
        }

        private BaseTask[] SortTaskOrder(IEnumerable<BaseTask> tasks)
        {
            float camZ = mainCamera.transform.position.z;
            return tasks.OrderBy(task => task.transform.position.z - camZ).ToArray();
        }

        public void Start()
        {
            //ゲーム開始時に画面内に存在するタスクの進捗バー表示
            foreach (BaseTask task in tasks)
            {
                Vector3 viewPos = mainCamera.WorldToViewportPoint(task.transform.position);

                if (IsPassed(viewPos))
                {
                    head++;
                    continue;
                }

                if (!IsAhead(viewPos))
                {
                    TaskProgressView view = taskProgressPool.GetProgressView(task.transform);
                    activeViews.Enqueue((task, view));
                    tail++;
                }
            }
        }

        public void Tick()
        {
            //最も近いタスクと遠いタスクのカメラ外検知
            UpdateHead();
            UpdateTail();

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

        void UpdateHead()
        {
            if (head >= tasks.Length)
                return;

            Transform headTransform = tasks[head].transform;
            Vector3 viewPos = mainCamera.WorldToViewportPoint(headTransform.position);

            if (IsPassed(viewPos))
            {
                (BaseTask task, TaskProgressView view) element = activeViews.Dequeue();
                taskProgressPool.ReleaseProgressView(element.view);
                head++;
            }
        }

        void UpdateTail()
        {
            if (tail >= tasks.Length)
                return;

            BaseTask task = tasks[tail];
            Vector3 viewPos = mainCamera.WorldToViewportPoint(task.transform.position);

            if (!IsAhead(viewPos))
            {
                TaskProgressView view = taskProgressPool.GetProgressView(task.transform);
                activeViews.Enqueue((task, view));
                tail++;
            }
        }


        bool IsAhead(Vector4 viewPos)
        {
            return viewPos.y > 1;
        }

        bool IsPassed(Vector3 viewPos)
        {
            return viewPos.y < -0.1;
        }
    }
}