using System.Spawner;
using GameSystem;
using VContainer;
using VContainer.Unity;

namespace System
{
    public class GameRouter : IStartable
    {
        private readonly SpawnParam spawnParam;
        private readonly WorkerSpawner workerSpawner;

        [Inject]
        public GameRouter(SpawnParam spawnParam,WorkerSpawner workerSpawner)
        {
            this.spawnParam = spawnParam;
            this.workerSpawner = workerSpawner;
        }

        public void Start()
        {
            workerSpawner.Spawn(spawnParam.SpawnCount);
        }
    }
}