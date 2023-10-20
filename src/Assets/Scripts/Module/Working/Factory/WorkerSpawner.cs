using System;
using Module.Working.State;
using UnityEngine;
using UnityEngine.AI;
using VContainer;
using Wanna.DebugEx;

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

            int layerMask = 1 << LayerMask.NameToLayer("Stage");

            if (Physics.Raycast(Vector3.up * 100f, Vector3.down, out RaycastHit hitInfo, 1000f, layerMask))
            {
                Vector3 position = hitInfo.point + new Vector3(0f, 0.5f, 0f);

                foreach (var worker in addedWorkers)
                {
                    WorkerCreateModel createModel = new WorkerCreateModel(WorkerState.Idle, position,
                        spawnPoint.transform);
                    InitWorker(worker, createModel);
                }
            }
            else
            {
                DebugEx.LogWarning("設置できるポイントがありません。");
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