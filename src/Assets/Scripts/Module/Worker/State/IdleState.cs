using UnityEngine;

namespace Module.Working.State
{
    public class IdleState : IWorkerState
    {
        private static readonly int IsFollowing = Animator.StringToHash("Following");
        private static readonly int IsWorking = Animator.StringToHash("Working");
        private static readonly int IsDead = Animator.StringToHash("Dead");

        // ReSharper disable once NotAccessedField.Local
        private readonly Worker worker;
        private readonly Animator workerAnimator;

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
            workerAnimator.SetBool(IsDead, false);
        }

        public void OnStop()
        {
        }

        public void Update()
        {
        }

        public void Dispose()
        {
        }
    }
}