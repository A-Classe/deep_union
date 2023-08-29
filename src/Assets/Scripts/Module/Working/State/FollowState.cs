using UnityEngine;
using UnityEngine.AI;

namespace Module.Working.State
{
    public class FollowState : IWorkerState
    {
        private readonly NavMeshAgent navMeshAgent;
        private readonly Worker worker;
        private readonly Animator workerAnimator;

        private static readonly int IsFollowing = Animator.StringToHash("Following");

        public FollowState(Worker worker)
        {
            this.worker = worker;
            navMeshAgent = worker.GetComponent<NavMeshAgent>();
            workerAnimator = worker.animator;
        }

        public WorkerState WorkerState => WorkerState.Following;

        private bool isMoving;

        public void OnStart() { }

        public void OnStop()
        {
            workerAnimator.SetBool(IsFollowing, false);
        }

        public void Update()
        {
            navMeshAgent.SetDestination(worker.Target.position + worker.Offset);

            //リーダーに追いついたとき
            if (IsStopped())
            {
                isMoving = false;
                workerAnimator.SetBool(IsFollowing, false);
            }
            //リーダーにリーダーを追いかけ始めたとき
            else if (IsMoved())
            {
                isMoving = true;
                workerAnimator.SetBool(IsFollowing, true);
            }
        }

        bool IsStopped()
        {
            return isMoving && navMeshAgent.velocity.sqrMagnitude == 0f;
        }

        bool IsMoved()
        {
            return !isMoving && navMeshAgent.velocity.sqrMagnitude > 0f;
        }

        public void Dispose() { }
    }
}