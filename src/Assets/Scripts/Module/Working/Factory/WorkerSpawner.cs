using System;
using Module.Working.State;
using UnityEngine;
using VContainer;
using Random = UnityEngine.Random;

namespace Module.Working.Factory
{
    public class WorkerSpawner
    {
        private readonly WorkerAgent workerAgent;
        private readonly SpawnPoint spawnPoint;
        private readonly SpawnParam spawnParam;

        [Inject]
        public WorkerSpawner(WorkerAgent workerAgent, SpawnPoint spawnPoint, SpawnParam spawnParam)
        {
            this.workerAgent = workerAgent;
            this.spawnPoint = spawnPoint;
            this.spawnParam = spawnParam;
        }

        public Span<Worker> Spawn(int spawnCount)
        {
            Span<Worker> addedWorkers = workerAgent.Add(spawnCount);

            foreach (Worker worker in addedWorkers)
            {
                //円状のランダム座標を取得
                Vector3 scaleTarget = new Vector3(spawnParam.SpawnRange, 0f, spawnParam.SpawnRange);
                Vector3 spawnPosition = Vector3.Scale(Random.insideUnitSphere, scaleTarget);

                //Workerの初期化
                WorkerCreateModel createModel = new WorkerCreateModel(WorkerState.Idle, spawnPosition, spawnPoint.transform);
                InitWorker(worker, createModel);
            }

            return addedWorkers;
        }

        void InitWorker(Worker worker, WorkerCreateModel createModel)
        {
            Transform transform = worker.transform;

            transform.position = createModel.Position;
            transform.SetParent(createModel.Parent);

            worker.OnSpawn(createModel.SpawnPoint);
            worker.SetWorkerState(createModel.State);
        }
    }
}