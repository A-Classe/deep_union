using System;
using System.Collections.Generic;
using System.Linq;
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
            Vector3[] spawnPoints = spawnPoint.GetSpawnPoints(spawnCount).ToArray();

            for (int i = 0; i < addedWorkers.Length; i++)
            {
                WorkerCreateModel createModel = new WorkerCreateModel(WorkerState.Idle, spawnPoints[i], spawnPoint.transform);
                InitWorker(addedWorkers[i], createModel);
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