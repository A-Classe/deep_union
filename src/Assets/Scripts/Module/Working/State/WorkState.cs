using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;
using Wanna.DebugEx;

namespace Module.Working.State
{
    public class WorkState : IWorkerState
    {
        private readonly NavMeshAgent navMeshAgent;
        private readonly Worker worker;
        private readonly Animator workerAnimator;

        private static readonly int IsFollowing = Animator.StringToHash("Following");
        private static readonly int IsWorking = Animator.StringToHash("Working");

        private CancellationTokenSource cTokenSource;

        public WorkState(Worker worker)
        {
            this.worker = worker;
            navMeshAgent = worker.GetComponent<NavMeshAgent>();
            workerAnimator = worker.animator;
        }

        public WorkerState WorkerState => WorkerState.Working;

        public void OnStart()
        {
            cTokenSource?.Dispose();
            cTokenSource = new CancellationTokenSource();

            SequenceWork(cTokenSource.Token).Forget();
        }

        async UniTaskVoid SequenceWork(CancellationToken cancellationToken)
        {
            workerAnimator.SetBool(IsFollowing, true);

            await UniTask.WaitUntil(IsArrived, cancellationToken: cancellationToken);

            workerAnimator.SetBool(IsFollowing, false);
            workerAnimator.SetBool(IsWorking, true);
            navMeshAgent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
        }

        private bool IsArrived()
        {
            return navMeshAgent != null && navMeshAgent.remainingDistance == 0f;
        }

        public void OnStop()
        {
            Dispose();
            workerAnimator.SetBool(IsWorking, false);
            navMeshAgent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
        }

        public void Update()
        {
            navMeshAgent.SetDestination(worker.Target.position);
        }

        public void Dispose()
        {
            cTokenSource?.Cancel();
            cTokenSource?.Dispose();
            cTokenSource = null;
        }
    }
}