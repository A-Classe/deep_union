using UnityEngine;
using UnityEngine.AI;

namespace Module.Working.State
{
    public class FollowState : IWorkerState
    {
        public WorkerState WorkerState => WorkerState.Following;

        private readonly Worker worker;
        private readonly Vector3 offset;
        private readonly NavMeshAgent navMeshAgent;

        public FollowState(Worker worker, Vector3 spawnPos)
        {
            this.worker = worker;
            this.offset = spawnPos - worker.transform.position;
            navMeshAgent = worker.GetComponent<NavMeshAgent>();
        }

        public void Update()
        {
            navMeshAgent.SetDestination(worker.Target.position + offset);
        }
    }
}