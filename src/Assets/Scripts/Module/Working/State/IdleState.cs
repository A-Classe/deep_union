namespace Module.Working.State
{
    public class IdleState : IWorkerState
    {
        // ReSharper disable once NotAccessedField.Local
        private readonly Worker worker;

        public IdleState(Worker worker)
        {
            this.worker = worker;
        }

        public WorkerState WorkerState => WorkerState.Idle;

        public void Update() { }
    }
}