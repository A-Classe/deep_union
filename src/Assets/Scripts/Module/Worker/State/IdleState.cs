namespace Module.Worker.State
{
    public class IdleState : IWorkerState
    {
        private readonly Worker worker;
        public WorkerState WorkerState => WorkerState.Idle;

        public IdleState(Worker worker)
        {
            this.worker = worker;
        }

        public void Update() { }
    }
}