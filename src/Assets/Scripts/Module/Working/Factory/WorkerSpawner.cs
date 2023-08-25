using System;
using System.Collections.Generic;
using System.Linq;
using Module.Working.State;
using UnityEngine;
using UnityEngine.AI;
using VContainer;
using Random = UnityEngine.Random;

namespace Module.Working.Factory
{
    /// <summary>
    /// ワーカーをスポーンするクラス
    /// </summary>
    public class WorkerSpawner
    {
        private readonly WorkerAgent workerAgent;
        private readonly SpawnPoint spawnPoint;

        [Inject]
        public WorkerSpawner(WorkerAgent workerAgent, SpawnPoint spawnPoint)
        {
            this.workerAgent = workerAgent;
            this.spawnPoint = spawnPoint;
        }

        /// <summary>
        /// 指定した数のワーカーをスポーンします
        /// </summary>
        /// <param name="spawnCount">スポーンするワーカーの数</param>
        /// <returns>スポーンしたワーカーのコレクション</returns>
        public Span<Worker> Spawn(int spawnCount)
        {
            Span<Worker> addedWorkers = workerAgent.Add(spawnCount);
            Span<Vector3> spawnPoints = spawnPoint.GetSpawnPoints(spawnCount);

            for (int i = 0; i < addedWorkers.Length; i++)
            {
                WorkerCreateModel createModel = new WorkerCreateModel(WorkerState.Idle, spawnPoints[i], spawnPoint.transform);
                InitWorker(addedWorkers[i], createModel);
            }

            return addedWorkers;
        }

        void InitWorker(Worker worker, WorkerCreateModel createModel)
        {
            worker.transform.SetParent(createModel.Parent);
            NavMeshAgent agent = worker.GetComponent<NavMeshAgent>();
            agent.Warp(createModel.Position);

            worker.OnSpawn();
            worker.SetWorkerState(createModel.State);
        }
    }
}