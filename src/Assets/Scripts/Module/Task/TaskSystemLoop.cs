using UnityEngine;
using VContainer;
using VContainer.Unity;
using Wanna.DebugEx;

namespace Module.Task
{
    /// <summary>
    ///     タスクのゲームループを行うクラス
    /// </summary>
    public class TaskSystemLoop : IStartable, ITickable
    {
        private readonly ITaskSystem[] taskSystems;
        private readonly IObjectResolver container;

        [Inject]
        public TaskSystemLoop(IObjectResolver container)
        {
            taskSystems = TaskUtil.FindSceneTasks<ITaskSystem>();
            this.container = container;
        }

        public void Start()
        {
            //タスクの初期化
            foreach (var taskSystem in taskSystems)
            {
                taskSystem.Initialize(container);
            }
        }

        public void Tick()
        {
            var delta = Time.deltaTime;

            //タスクの更新
            foreach (var taskSystem in taskSystems)
            {
                if (taskSystem.State != TaskState.InProgress)
                    continue;

                taskSystem.TaskSystemUpdate(delta);
            }
        }
    }
}