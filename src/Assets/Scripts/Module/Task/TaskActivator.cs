using System;
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

        [Inject]
        public TaskActivator(GameParam gameParam)
        {
            this.gameParam = gameParam;
            mainCamera = Camera.main;

            var camZ = mainCamera.transform.position.z;
            tasks = TaskUtil.FindSceneTasks<BaseTask>().OrderBy(task => task.transform.position.z - camZ).ToArray();
            tail = tasks.Length - 1;
        }

        public event Action OnTaskCreated;
        public event Action<BaseTask> OnTaskActivated;
        public event Action<BaseTask> OnTaskDeactivated;

        public void Start()
        {
            foreach (var task in tasks)
                if (IsPassed(task.transform.position))
                {
                    task.Disable();
                    head++;
                }
                else if (IsAhead(task.transform.position))
                {
                    task.Disable();
                    tail--;
                }

            OnTaskCreated?.Invoke();
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

        private void UpdateHead()
        {
            if (head >= tasks.Length)
                return;

            var task = tasks[head];

            if (IsPassed(task.transform.position))
            {
                OnTaskDeactivated?.Invoke(task);
                task.Disable();
                head++;
            }
        }

        private void UpdateTail()
        {
            if (tail >= tasks.Length)
                return;

            var task = tasks[tail];

            if (!IsAhead(task.transform.position))
            {
                task.Enable();
                OnTaskActivated?.Invoke(task);
                tail++;
            }
        }

        private bool IsAhead(Vector3 taskPos)
        {
            return taskPos.z - mainCamera.transform.position.z > gameParam.ActivateTaskRange;
        }

        private bool IsPassed(Vector3 taskPos)
        {
            return mainCamera.transform.position.z - taskPos.z > gameParam.DeactivateTaskRange;
        }
    }
}