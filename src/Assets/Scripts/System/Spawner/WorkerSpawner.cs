using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using Worker;
using Worker.Factory;
using Random = UnityEngine.Random;

namespace GameSystem
{
    public class WorkerSpawner
    {
        private readonly WorkerFactory factory;
        private readonly SpawnPoint spawnPoint;

        private readonly List<TaskWorker> taskWorkers;
        public IReadOnlyList<TaskWorker> TaskWorkers => taskWorkers;

        [Inject]
        public WorkerSpawner(WorkerFactory factory, SpawnPoint spawnPoint)
        {
            this.factory = factory;
            this.spawnPoint = spawnPoint;

            taskWorkers = new List<TaskWorker>();
        }

        public void Spawn(int spawnCount)
        {
            for (int i = 0; i < spawnCount; i++)
            {
                //円状にランダムに配置
                Vector3 scaleTarget = new Vector3(spawnPoint.SpawnRange, 0f, spawnPoint.SpawnRange);
                Vector3 spawnPos = Vector3.Scale(Random.insideUnitSphere, scaleTarget);

                //Factoryに渡してWorkerを生成
                WorkerCreateModel createModel = new WorkerCreateModel(WorkerState.Stay, spawnPos, spawnPoint.transform);
                TaskWorker worker = factory.CreateWorker(createModel);

                taskWorkers.Add(worker);
            }
        }
    }
}