using UnityEngine;
using UnityEngine.AI;
using Wanna.DebugEx;

namespace Module.Working.State
{
    public class FollowState : IWorkerState
    {
        private static readonly int IsFollowing = Animator.StringToHash("Following");
        private readonly NavMeshAgent navMeshAgent;
        private readonly Worker worker;
        private readonly Animator workerAnimator;
        private Vector3 currentPos;

        private bool isMoving;

        private Vector3 prevPos;

        public FollowState(Worker worker)
        {
            this.worker = worker;
            navMeshAgent = worker.GetComponent<NavMeshAgent>();
            workerAnimator = worker.animator;

            prevPos = worker.transform.position;
            currentPos = prevPos;
        }

        public WorkerState WorkerState => WorkerState.Following;

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

        public void Dispose()
        {
        }

        private bool IsStopped()
        {
            return isMoving && navMeshAgent.velocity.sqrMagnitude == 0f && !worker.IsWorldMoving;
        }

        private bool IsMoved()
        {
            return !isMoving && navMeshAgent.velocity.sqrMagnitude > 0f;
        }
    }
}