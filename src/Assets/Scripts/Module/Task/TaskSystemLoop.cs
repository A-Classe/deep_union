using System;
using Core.Debug;
using UnityDebugSheet.Runtime.Core.Scripts;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using Wanna.DebugEx;

namespace Module.Task
{
    /// <summary>
    ///     タスクのゲームループを行うクラス
    /// </summary>
    public class TaskSystemLoop : IStartable, ITickable, IDisposable
    {
        private readonly ITaskSystem[] taskSystems;
        private readonly IObjectResolver container;
        private readonly TaskDebugSheet taskDebugSheet;

        [Inject]
        public TaskSystemLoop(IObjectResolver container)
        {
            taskDebugSheet = new TaskDebugSheet();
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
            
            taskDebugSheet.Update();
        }

        public void Dispose()
        {
            taskDebugSheet.Dispose();
        }
    }
}