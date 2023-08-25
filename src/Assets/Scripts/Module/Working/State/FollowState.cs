using UnityEngine;
using UnityEngine.AI;

namespace Module.Working.State
{
    public class FollowState : IWorkerState
    {
        private readonly NavMeshAgent navMeshAgent;
        private readonly Vector3 offset;

        private readonly Worker worker;

        public FollowState(Worker worker, Vector3 spawnPos)
        {
            this.worker = worker;
            offset = spawnPos - worker.transform.position;
            navMeshAgent = worker.GetComponent<NavMeshAgent>();
        }

        public WorkerState WorkerState => WorkerState.Following;

        public void Update()
        {
            navMeshAgent.SetDestination(worker.Target.position + offset);
        }
    }
}