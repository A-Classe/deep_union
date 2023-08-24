using System;
using Core.Utility;
using Module.Working;
using Module.Working.Controller;
using Module.Working.Factory;
using VContainer;
using VContainer.Unity;

namespace GameMain
{
    public class GameRouter : IStartable
    {
        private readonly SpawnParam spawnParam;
        private readonly WorkerSpawner workerSpawner;
        private readonly WorkerController workerController;

        [Inject]
        public GameRouter(SpawnParam spawnParam, WorkerSpawner workerSpawner, WorkerController workerController)
        {
            this.spawnParam = spawnParam;
            this.workerSpawner = workerSpawner;
            this.workerController = workerController;
        }

        public void Start()
        {
            Span<Worker> workers = workerSpawner.Spawn(spawnParam.SpawnCount);

            foreach (Worker worker in workers)
            {
                workerController.EnqueueWorker(worker);
            }
        }
    }
}