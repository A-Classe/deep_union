using System;
using Module.Working.State;
using UnityEngine;
using UnityEngine.AI;
using VContainer;

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
        public ReadOnlySpan<Worker> Spawn(int spawnCount)
        {
            var addedWorkers = workerAgent.Add(spawnCount);

            foreach (var worker in addedWorkers)
            {
                WorkerCreateModel createModel = new WorkerCreateModel(WorkerState.Idle, Vector3.zero, spawnPoint.transform);
                InitWorker(worker, createModel);
            }

            return addedWorkers;
        }

        void InitWorker(Worker worker, WorkerCreateModel createModel)
        {
            worker.transform.SetParent(createModel.Parent);
            NavMeshAgent agent = worker.GetComponent<NavMeshAgent>();
            agent.Warp(createModel.Position);

            worker.SetWorkerState(createModel.State);
        }
    }
}