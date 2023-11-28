using System;
using System.Collections;
using System.Collections.Generic;
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
        private WorkerAgent workerAgent;
        
        public override void Initialize(IObjectResolver container)
        {
            workerAgent = container.Resolve<WorkerAgent>();
                
            ForceComplete();
        }

        private void CreateWorkers()
        {
            IReadOnlyList<AssignPoint> assignPoints = assignableArea.AssignPoints;
            ReadOnlySpan<Worker> workers = workerAgent.Add(assignPoints.Count);
            
            foreach (Worker worker in workers)
            {
                assignableArea.
            }
        }
    }
}