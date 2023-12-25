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
        private readonly Memory<BaseTask> tasksHalf1;
        private readonly Memory<BaseTask> tasksHalf2;

        private bool isFirstFrame = true;

        [Inject]
        public TaskActivator(GameParam gameParam)
        {
            this.gameParam = gameParam;
            mainCamera = Camera.main;

            var camZ = mainCamera.transform.position.z;
            
            //タスクを２つに分割する
            tasks = TaskUtil.FindSceneTasks<BaseTask>().OrderBy(task => task.transform.position.z - camZ).ToArray();
            tasksHalf1 = tasks.AsMemory(0, tasks.Length / 2);
            tasksHalf2 = tasks.AsMemory(tasksHalf1.Length, tasks.Length - tasksHalf1.Length);
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
                    OnTaskInitialized?.Invoke(tasks.AsMemory(0, i + 1));
                    break;
                }

                task.Disable();
            }
        }

        public ReadOnlySpan<BaseTask> GetAllTasks()
        {
            return tasks.AsSpan();
        }

        private void ActivationLoop(Span<BaseTask> baseTasks)
        {
            foreach (BaseTask task in baseTasks)
            {
                if (task.gameObject.activeSelf)
                {
                    DisableTask(task);
                }
                else
                {
                    EnableTask(task);
                }
            }
        }

        public void Tick()
        {
            //1フレームで半分のタスクを判定する
            Span<BaseTask> tasksHalf = isFirstFrame ? tasksHalf1.Span : tasksHalf2.Span; 
            ActivationLoop(tasksHalf);
            isFirstFrame = !isFirstFrame;
        }

        private void EnableTask(BaseTask task)
        {
            if (IsInsideCamera(task.transform.position))
            {
                task.Enable();
                OnTaskActivated?.Invoke(task);
            }
        }

        private void DisableTask(BaseTask task)
        {
            if (!IsInsideCamera(task.transform.position))
            {
                OnTaskDeactivated?.Invoke(task);
                task.Disable();
            }
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