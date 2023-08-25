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

        private WorkerController workerController;

        public override void Initialize(IObjectResolver container)
        {
            workerController = container.Resolve<WorkerController>();

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
                workerController.EnqueueWorker(worker);
            }
        }
    }
}