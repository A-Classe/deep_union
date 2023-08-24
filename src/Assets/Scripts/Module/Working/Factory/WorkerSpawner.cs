using System;
using Module.Working.State;
using UnityEngine;
using VContainer;
using Random = UnityEngine.Random;

namespace Module.Working.Factory
{
    public class WorkerSpawner
    {
        private readonly SpawnParam spawnParam;
        private readonly SpawnPoint spawnPoint;
        private readonly WorkerAgent workerAgent;

        [Inject]
        public WorkerSpawner(WorkerAgent workerAgent, SpawnPoint spawnPoint, SpawnParam spawnParam)
        {
            this.workerAgent = workerAgent;
            this.spawnPoint = spawnPoint;
            this.spawnParam = spawnParam;
        }

        public Span<Worker> Spawn(int spawnCount)
        {
            var addedWorkers = workerAgent.Add(spawnCount);

            foreach (var worker in addedWorkers)
            {
                //円状のランダム座標を取得
                var scaleTarget = new Vector3(spawnParam.SpawnRange, 0f, spawnParam.SpawnRange);
                var spawnPosition = Vector3.Scale(Random.insideUnitSphere, scaleTarget);

                //Workerの初期化
                var createModel = new WorkerCreateModel(WorkerState.Idle, spawnPosition, spawnPoint.transform);
                InitWorker(worker, createModel);
            }

            return addedWorkers;
        }

        private void InitWorker(Worker worker, WorkerCreateModel createModel)
        {
            var transform = worker.transform;

            transform.position = createModel.Position;
            transform.SetParent(createModel.Parent);

            worker.OnSpawn(createModel.SpawnPoint);
            worker.SetWorkerState(createModel.State);
        }
    }
}