using Core.Utility;
using Module.Worker;
using Module.Worker.Controller;
using Module.Worker.Factory;
using VContainer;
using VContainer.Unity;

namespace System
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
            ListSegment<Worker> workers = workerSpawner.Spawn(spawnParam.SpawnCount);
            workerController.SetWorkers(workers);
        }
    }
}