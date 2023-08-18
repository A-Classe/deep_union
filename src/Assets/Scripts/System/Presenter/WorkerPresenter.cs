using Controller;
using GameSystem;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using Module.Worker;

namespace System
{
    public class WorkerPresenter : ITickable
    {
        private readonly WorkerController workerController;
        private readonly WorkerSpawner workerSpawner;

        [Inject]
        public WorkerPresenter(WorkerController workerController, WorkerSpawner workerSpawner)
        {
            this.workerController = workerController;
            this.workerSpawner = workerSpawner;
        }

        public void Tick()
        {
            //Workerの移動目標を更新
            UpdateFollowPosition();
        }

        void UpdateFollowPosition()
        {
            Vector3 position = workerController.GetPosition();
            foreach (Module.Worker.Worker worker in workerSpawner.TaskWorkers)
            {
                worker.SetFollowPoint(position);
            }
        }
    }
}