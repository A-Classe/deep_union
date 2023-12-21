using System;
using Module.Assignment.Component;
using Module.Task;
using Module.Working;
using Module.Working.State;
using UnityEngine;
using VContainer;

namespace Module.Extension.Task
{
    public class WorkerCollectTask : BaseTask
    {
        [SerializeField] private AssignableArea assignableArea;
        [SerializeField] private Transform assignPointsParent;
        private WorkerAgent workerAgent;

        public override void Initialize(IObjectResolver container)
        {
            workerAgent = container.Resolve<WorkerAgent>();
            ForceComplete();
            CreateWorkers();
        }

        private void CreateWorkers()
        {
            AssignPoint[] assignPoints = assignPointsParent.GetComponentsInChildren<AssignPoint>();
            ReadOnlySpan<Worker> workers = workerAgent.Add(assignPoints.Length);

            for (int i = 0; i < assignPoints.Length; i++)
            {
                AssignPoint assignPoint = assignPoints[i];

                Vector3 position = assignPoint.transform.position;
                workers[i].Teleport(position);
            }


            foreach (Worker worker in workers)
            {
                assignableArea.AddWorker(worker);
                worker.SetWorkerState(WorkerState.Following);
            }
        }
    }
}