using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Module.Task
{
    public class TaskActivator
    {
        private readonly Camera mainCamera;
        private readonly BaseTask[] tasks;

        private int head;
        private int tail;

        public event Action<BaseTask> OnTaskActivated;
        public event Action<BaseTask> OnTaskDeactivated;

        public TaskActivator()
        {
            mainCamera = Camera.main;
            tasks = SortTaskOrder(TaskUtil.FindSceneTasks<BaseTask>());

            Initialize();
        }

        private BaseTask[] SortTaskOrder(IEnumerable<BaseTask> tasks)
        {
            float camZ = mainCamera.transform.position.z;
            return tasks.OrderBy(task => task.transform.position.z - camZ).ToArray();
        }

        private void Initialize()
        {
            foreach (BaseTask task in tasks)
            {
                Vector3 viewPos = mainCamera.WorldToViewportPoint(task.transform.position);

                if (IsPassed(viewPos))
                {
                    head++;
                }
                else if (!IsAhead(viewPos))
                {
                    tail++;
                }
            }
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
                OnTaskActivated?.Invoke(task);
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