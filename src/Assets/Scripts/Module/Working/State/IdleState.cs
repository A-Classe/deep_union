namespace Module.Working.State
{
    public class IdleState : IWorkerState
    {
        private readonly Worker worker;

        public IdleState(Worker worker)
        {
            this.worker = worker;
        }

        public WorkerState WorkerState => WorkerState.Idle;

        public void Update() { }
    }
}