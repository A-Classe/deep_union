namespace Module.Working.State
{
    public interface IWorkerState
    {
        WorkerState WorkerState { get; }

        void OnStart();
        void OnStop();
        void Update();
        void Dispose();
    }
}