namespace Module.Working.State
{
    public interface IWorkerState
    {
        WorkerState WorkerState { get; }

        void Update();
    }
}