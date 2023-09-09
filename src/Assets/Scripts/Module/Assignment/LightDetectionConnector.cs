using System.Collections.Generic;
using GameMain.System;
using Module.Task;
using Module.Working;
using Module.Working.Controller;
using VContainer;
using VContainer.Unity;
using Wanna.DebugEx;

namespace Module.Assignment
{
    public class LightDetectionConnector : IInitializable, ITickable
    {
        private readonly LightDetector lightDetector;
        private readonly TaskActivator taskActivator;
        private readonly WorkerController workerController;
        private readonly Dictionary<Worker, AssignableArea> assignments;

        [Inject]
        public LightDetectionConnector(LightDetector lightDetector, TaskActivator taskActivator, WorkerController workerController)
        {
            this.lightDetector = lightDetector;
            this.taskActivator = taskActivator;
            this.workerController = workerController;

            assignments = new Dictionary<Worker, AssignableArea>();
        }

        public void Initialize()
        {
            //プレイヤーのアサインエリアを登録
            lightDetector.AssignableAreas.Add(workerController.GetComponentInChildren<AssignableArea>());

            //タスクのアサインエリアを登録
            foreach (BaseTask task in taskActivator.GetActiveTasks())
            {
                lightDetector.AssignableAreas.Add(task.GetComponentInChildren<AssignableArea>());
            }

            taskActivator.OnTaskActivated += task =>
            {
                lightDetector.AssignableAreas.Add(task.GetComponentInChildren<AssignableArea>());
            };

            taskActivator.OnTaskDeactivated += task =>
            {
                //0番目はプレイヤー専用なので、1番目を削除
                lightDetector.AssignableAreas.RemoveAt(1);
            };

            lightDetector.OnAssignableAreaDetected += OnAssignableAreaDetected;
        }

        private void OnAssignableAreaDetected(Worker worker, AssignableArea assignableArea)
        {
            if (assignments.Remove(worker, out AssignableArea area))
            {
                area.OnWorkerExit_Internal(worker);
            }

            assignableArea.OnWorkerEnter_Internal(worker);
            assignments.Add(worker, assignableArea);
        }

        public void Tick()
        {
            lightDetector.UpdateDetection();
        }
    }
}