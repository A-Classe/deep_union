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

        private Vector3 prevPos;
        private Vector3 currentPos;

        public FollowState(Worker worker)
        {
            this.worker = worker;
            navMeshAgent = worker.GetComponent<NavMeshAgent>();
            workerAnimator = worker.animator;

            prevPos = worker.transform.position;
            currentPos = prevPos;
        }

        public WorkerState WorkerState => WorkerState.Following;

        private bool isMoving;

        public void OnStart()
        {
            navMeshAgent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
            workerAnimator.SetBool(IsFollowing, true);
        }

        public void OnStop()
        {
            workerAnimator.SetBool(IsFollowing, false);
        }

        public void Update()
        {
            if (navMeshAgent.pathStatus == NavMeshPathStatus.PathInvalid)
                return;

            prevPos = currentPos;
            currentPos = worker.transform.position;

            navMeshAgent.SetDestination(worker.Target.position);

            //リーダーに追いついたとき
            if (IsStopped())
            {
                isMoving = false;
                workerAnimator.SetBool(IsFollowing, false);
            }
            //リーダーを追いかけ始めたとき
            else if (IsMoved())
            {
                isMoving = true;
                workerAnimator.SetBool(IsFollowing, true);
            }
        }

        bool IsStopped()
        {
            return isMoving && navMeshAgent.velocity.sqrMagnitude == 0f && !worker.IsWorldMoving;
        }

        bool IsMoved()
        {
            return !isMoving && navMeshAgent.velocity.sqrMagnitude > 0f;
        }

        public void Dispose() { }
    }
}