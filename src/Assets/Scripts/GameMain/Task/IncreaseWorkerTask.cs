using System.Collections.Generic;
using Module.Task;
using Module.Working;
using Module.Working.Controller;
using UnityEngine;
using VContainer;

namespace GameMain.Task
{
    public class IncreaseWorkerTask : BaseTask
    {
        [Header("増やすワーカーのリスト")]
        [SerializeField]
        private List<Worker> imprisonedWorkers;

        private LeadPointConnector leadPointConnector;

        public override void Initialize(IObjectResolver container)
        {
            leadPointConnector = container.Resolve<LeadPointConnector>();

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
            }

            foreach (Worker worker in imprisonedWorkers)
            {
                leadPointConnector.AddWorker(worker);
            }
        }
    }
}