using System.Collections.Generic;
using Module.Assignment;
using Module.Assignment.Component;
using Module.Task;
using Module.Working;
using Module.Working.Controller;
using Module.Working.Factory;
using Module.Working.State;
using UnityEngine;
using VContainer;

namespace GameMain.Task
{
    public class IncreaseWorkerTask : BaseTask
    {
        [Header("増やすワーカーのリスト")]
        [SerializeField]
        private List<Worker> imprisonedWorkers;

        private WorkerAgent workerAgent;
        private LeaderAssignableArea leaderAssignableArea;
        private SpawnPoint spawnPoint;

        public override void Initialize(IObjectResolver container)
        {
            workerAgent = container.Resolve<WorkerAgent>();
            leaderAssignableArea = container.Resolve<LeaderAssignableArea>();
            spawnPoint = container.Resolve<SpawnPoint>();

            foreach (Worker worker in imprisonedWorkers)
            {
                worker.SetLockState(true);
            }
        }

        protected override void OnComplete()
        {
            foreach (Worker worker in imprisonedWorkers)
            {
                worker.SetLockState(false);
                workerAgent.AddActiveWorker(worker);

                leaderAssignableArea.AssignableArea.AddWorker(worker);
                worker.transform.SetParent(spawnPoint.transform);
            }
        }
    }
}