using Module.Working.Controller;
using Module.Working.Factory;
using VContainer;
using VContainer.Unity;

namespace GameMain
{
    public class GameRouter : IStartable
    {
        private readonly SpawnParam spawnParam;
        private readonly WorkerController workerController;
        private readonly WorkerSpawner workerSpawner;

        [Inject]
        public GameRouter(SpawnParam spawnParam, WorkerSpawner workerSpawner, WorkerController workerController)
        {
            this.spawnParam = spawnParam;
            this.workerSpawner = workerSpawner;
            this.workerController = workerController;
        }

        public void Start()
        {
            var workers = workerSpawner.Spawn(spawnParam.SpawnCount);

            foreach (var worker in workers) workerController.EnqueueWorker(worker);
        }
    }
}