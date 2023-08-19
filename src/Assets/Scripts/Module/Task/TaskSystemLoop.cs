using System.Collections.Generic;
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
        private readonly List<ITaskSystem> taskSystems = default;

        [Inject]
        public TaskSystemLoop()
        {
            taskSystems = new List<ITaskSystem>();
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
                taskSystem.Update(delta);
            }
        }
    }
}