using UnityEngine.AI;

namespace Module.Working.State
{
    public class WorkState : IWorkerState
    {
        private readonly NavMeshAgent navMeshAgent;

        private readonly Worker worker;

        public WorkState(Worker worker)
        {
            this.worker = worker;
            navMeshAgent = worker.GetComponent<NavMeshAgent>();
        }

        public WorkerState WorkerState => WorkerState.Working;

        public void Update()
        {
            navMeshAgent.SetDestination(worker.Target.position);
        }
    }
}