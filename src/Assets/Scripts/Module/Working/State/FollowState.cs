using UnityEngine;
using UnityEngine.AI;

namespace Module.Working.State
{
    public class FollowState : IWorkerState
    {
        private readonly NavMeshAgent navMeshAgent;
        private readonly Worker worker;

        public FollowState(Worker worker)
        {
            this.worker = worker;
            navMeshAgent = worker.GetComponent<NavMeshAgent>();
        }

        public WorkerState WorkerState => WorkerState.Following;

        public void Update()
        {
            navMeshAgent.SetDestination(worker.Target.position + worker.Offset);
        }
    }
}