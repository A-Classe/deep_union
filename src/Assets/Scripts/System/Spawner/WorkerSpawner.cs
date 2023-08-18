using System;
using System.Collections;
using System.Collections.Generic;
using System.Spawner;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using Module.Worker;
using Module.Worker.Factory;
using Random = UnityEngine.Random;

namespace GameSystem
{
    public class WorkerSpawner
    {
        private readonly WorkerFactory factory;
        private readonly SpawnPoint spawnPoint;
        private readonly SpawnParam spawnParam;

        private readonly List<Module.Worker.Worker> taskWorkers;
        public IReadOnlyList<Module.Worker.Worker> TaskWorkers => taskWorkers;

        [Inject]
        public WorkerSpawner(WorkerFactory factory, SpawnPoint spawnPoint, SpawnParam spawnParam)
        {
            this.factory = factory;
            this.spawnPoint = spawnPoint;
            this.spawnParam = spawnParam;

            taskWorkers = new List<Module.Worker.Worker>();
        }

        public void Spawn(int spawnCount)
        {
            for (int i = 0; i < spawnCount; i++)
            {
                //円状にランダムに配置
                Vector3 scaleTarget = new Vector3(spawnParam.SpawnRange, 0f, spawnParam.SpawnRange);
                Vector3 spawnPos = Vector3.Scale(Random.insideUnitSphere, scaleTarget);

                //Factoryに渡してWorkerを生成
                WorkerCreateModel createModel =
                    new WorkerCreateModel(WorkerState.Stay, spawnPoint.transform.position, spawnPos, spawnPoint.transform);
                Module.Worker.Worker worker = factory.CreateWorker(createModel);

                taskWorkers.Add(worker);
            }
        }
    }
}