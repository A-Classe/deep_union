using UnityEngine.AI;

namespace Module.Working.State
{
    public class WorkState : IWorkerState
    {
        public WorkerState WorkerState => WorkerState.Working;

        private readonly Worker worker;
        private readonly NavMeshAgent navMeshAgent;

        public WorkState(Worker worker)
        {
            this.worker = worker;
            navMeshAgent = worker.GetComponent<NavMeshAgent>();
        }

        public void Update()
        {
            navMeshAgent.SetDestination(worker.Target.position);
        }
    }
}