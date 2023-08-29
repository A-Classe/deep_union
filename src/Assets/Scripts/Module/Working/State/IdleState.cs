using UnityEngine;

namespace Module.Working.State
{
    public class IdleState : IWorkerState
    {
        // ReSharper disable once NotAccessedField.Local
        private readonly Worker worker;
        private readonly Animator workerAnimator;
        private static readonly int IsFollowing = Animator.StringToHash("Following");
        private static readonly int IsWorking = Animator.StringToHash("Working");

        public IdleState(Worker worker)
        {
            this.worker = worker;
            workerAnimator = worker.animator;
        }

        public WorkerState WorkerState => WorkerState.Idle;

        public void OnStart()
        {
            workerAnimator.SetBool(IsFollowing, false);
            workerAnimator.SetBool(IsWorking, false);
        }

        public void OnStop() { }

        public void Update() { }
        public void Dispose() { }
    }
}