using System;
using System.Collections.Generic;
using System.Linq;
using GameMain.Presenter;
using UnityEngine;
using VContainer;

namespace Module.Task
{
    public class TaskActivator
    {
        private readonly GameParam gameParam;
        private readonly Camera mainCamera;
        private readonly BaseTask[] tasks;

        private int head;
        private int tail;

        public event Action OnTaskCreated;
        public event Action<BaseTask> OnTaskActivated;
        public event Action<BaseTask> OnTaskDeactivated;

        [Inject]
        public TaskActivator(GameParam gameParam)
        {
            this.gameParam = gameParam;
            mainCamera = Camera.main;
            tasks = SortTaskOrder(TaskUtil.FindSceneTasks<BaseTask>());
        }

        public void Start()
        {
            foreach (BaseTask task in tasks)
            {
                Vector3 viewPos = mainCamera.WorldToViewportPoint(task.transform.position);

                if (IsPassed(viewPos))
                {
                    task.Disable();
                    head++;
                }
                else if (!IsAhead(viewPos))
                {
                    tail++;
                }
                else
                {
                    task.Disable();
                }
            }

            OnTaskCreated?.Invoke();
        }

        private BaseTask[] SortTaskOrder(IEnumerable<BaseTask> tasks)
        {
            float camZ = mainCamera.transform.position.z;
            return tasks.OrderBy(task => task.transform.position.z - camZ).ToArray();
        }

        public ReadOnlySpan<BaseTask> GetActiveTasks()
        {
            return tasks.AsSpan(head, tail);
        }

        public void Tick()
        {
            //最も近いタスクと遠いタスクのカメラ外検知
            UpdateHead();
            UpdateTail();
        }

        void UpdateHead()
        {
            if (head >= tasks.Length)
                return;

            BaseTask task = tasks[head];
            Vector3 viewPos = mainCamera.WorldToViewportPoint(task.transform.position);

            if (IsPassed(viewPos))
            {
                OnTaskDeactivated?.Invoke(task);
                task.Disable();
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
                task.Enable();
                OnTaskActivated?.Invoke(task);
                tail++;
            }
        }

        bool IsAhead(Vector4 viewPos)
        {
            return viewPos.y > gameParam.ActivateTaskRange;
        }

        bool IsPassed(Vector3 viewPos)
        {
            return viewPos.y < gameParam.DeactivateTaskRange;
        }
    }
}