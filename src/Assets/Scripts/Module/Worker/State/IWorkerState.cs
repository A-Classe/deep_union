using Module.Worker.Factory;

namespace Module.Worker.State
{
    public interface IWorkerState
    {
        WorkerState WorkerState { get; }

        void Update();
    }
}