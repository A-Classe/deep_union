using System;
using System.Linq;
using GameMain.Presenter;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;
using Wanna.DebugEx;

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
            head = 0;
        }

        public event Action<ReadOnlyMemory<BaseTask>> OnTaskInitialized;
        public event Action<BaseTask> OnTaskActivated;
        public event Action<BaseTask> OnTaskDeactivated;

        public void Start()
        {
            //最初のカメラ内の有効タスクの検出
            for (int i = tasks.Length - 1; i >= 0; i--)
            {
                BaseTask task = tasks[i];

                if (IsInsideCamera(task.transform.position))
                {
                    tail = i - 1;
                    break;
                }

                task.Disable();
            }

            OnTaskInitialized?.Invoke(tasks.AsMemory(head, tail + 1));
        }

        public ReadOnlySpan<BaseTask> GetAllTasks()
        {
            return tasks.AsSpan();
        }

        public void Tick()
        {
            if (Keyboard.current.f3Key.wasPressedThisFrame)
            {
                DebugEx.Log(tasks.AsMemory(head, tail + 1).ToArray().Select(src => src.name));
            }

            //カメラ内オブジェクトの検出
            DetectInsideTask();

            //カメラ外オブジェクトの検出
            DetectOutsideTask();
        }

        private void DetectInsideTask()
        {
            if (tail + 1 < tasks.Length)
            {
                BaseTask task = tasks[tail + 1];
                bool isInside = task.OmitActivator || TryEnableTask(task);

                if (isInside)
                {
                    tail++;
                }
            }

            if (head - 1 >= 0)
            {
                BaseTask task = tasks[head - 1];
                bool isInside = task.OmitActivator || TryEnableTask(task);

                if (isInside)
                {
                    head--;
                }
            }
        }

        private void DetectOutsideTask()
        {
            if (tail < tasks.Length)
            {
                BaseTask task = tasks[tail];
                bool isOutside = task.OmitActivator || TryDisableTask(task);

                if (isOutside)
                {
                    tail--;
                }
            }

            if (head >= 0)
            {
                BaseTask task = tasks[head];
                bool isOutside = task.OmitActivator || TryDisableTask(task);

                if (isOutside)
                {
                    head++;
                }
            }
        }

        private bool TryEnableTask(BaseTask task)
        {
            bool isInside = IsInsideCamera(task.transform.position);

            if (isInside)
            {
                task.Enable();
                OnTaskActivated?.Invoke(task);
            }

            return isInside;
        }

        private bool TryDisableTask(BaseTask task)
        {
            bool isOutside = !task.gameObject.activeSelf || !IsInsideCamera(task.transform.position);

            if (isOutside)
            {
                OnTaskDeactivated?.Invoke(task);
                task.Disable();
            }

            return isOutside;
        }

        public void ForceActivate(BaseTask task)
        {
            task.Enable();
            OnTaskActivated?.Invoke(task);
        }

        public void ForceDeactivate(BaseTask task)
        {
            OnTaskDeactivated?.Invoke(task);
            task.Disable();
        }

        private bool IsInsideCamera(Vector3 taskPos)
        {
            return Mathf.Abs(taskPos.z - mainCamera.transform.position.z) <= gameParam.ActivateTaskRange;
        }
    }
}