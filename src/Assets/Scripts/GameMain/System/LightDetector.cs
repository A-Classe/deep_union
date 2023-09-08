using System.Collections.Generic;
using GameMain.Presenter;
using Module.Task;
using Module.Working;
using Module.Working.Controller;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using VContainer;
using NotImplementedException = System.NotImplementedException;

namespace GameMain.System
{
    public class LightDetector
    {
        private readonly TaskActivator taskActivator;
        private readonly WorkerAgent workerAgent;
        private readonly GameParam gameParam;
        private readonly PhysicsScene physicsScene;
        private readonly int layerMask;
        private List<Transform> origins;

        private readonly Queue<BaseTask> activeTasks;

        [Inject]
        public LightDetector(TaskActivator taskActivator, WorkerAgent workerAgent, GameParam gameParam)
        {
            this.taskActivator = taskActivator;
            this.workerAgent = workerAgent;
            activeTasks = new Queue<BaseTask>();
        }

        private void InitializeTasks()
        {
            //既に有効になっているタスクを登録
            foreach (BaseTask task in taskActivator.GetActiveTasks())
            {
                activeTasks.Enqueue(task);
            }

            taskActivator.OnTaskActivated += task =>
            {
                activeTasks.Enqueue(task);
            };

            taskActivator.OnTaskActivated += task =>
            {
                activeTasks.Enqueue(task);
            };
        }

        public void UpdateDetection()
        {
            foreach (Worker worker in workerAgent.ActiveWorkers)
            {
                
            }
        }

        struct EllipseCollideJob:IJob
        {
            public void Execute()
            {
                
            }
        }
    }
}