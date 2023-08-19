using UnityEngine.AI;

namespace Module.Worker.State
{
    public class FollowState : IWorkerState
    {
        public WorkerState WorkerState => WorkerState.Following;

        private readonly Worker worker;
        private readonly NavMeshAgent navMeshAgent;

        public FollowState(Worker worker)
        {
            this.worker = worker;
            navMeshAgent = worker.GetComponent<NavMeshAgent>();
        }

        public void Update()
        {
            navMeshAgent.SetDestination(worker.FollowPoint);
        }
    }
}