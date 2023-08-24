using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Module.Task
{
    /// <summary>
    /// タスクのゲームループを行うクラス
    /// </summary>
    public class TaskSystemLoop : IStartable, ITickable
    {
        private readonly ITaskSystem[] taskSystems;

        [Inject]
        public TaskSystemLoop()
        {
            taskSystems = TaskUtil.FindSceneTasks<ITaskSystem>();
        }

        public void Start()
        {
            //タスクの初期化
            foreach (ITaskSystem taskSystem in taskSystems)
            {
                taskSystem.Initialize();
            }
        }

        public void Tick()
        {
            float delta = Time.deltaTime;

            //タスクの更新
            foreach (ITaskSystem taskSystem in taskSystems)
            {
                if (taskSystem.State != TaskState.InProgress)
                    continue;

                taskSystem.TaskSystemUpdate(delta);
            }
        }
    }
}