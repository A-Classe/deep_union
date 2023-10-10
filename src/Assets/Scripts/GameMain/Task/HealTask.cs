using System;
using Core.NavMesh;
using Cysharp.Threading.Tasks;
using Module.Assignment;
using Module.Assignment.Component;
using Module.Player.Controller;
using Module.Task;
using UnityEngine;
using UnityEngine.AI;
using VContainer;
using Wanna.DebugEx;

namespace GameMain.Task
{
    public class HealTask : BaseTask
    {
        [SerializeField] private NavMeshAgent navMeshAgent;
        [SerializeField] private AssignableArea assignableArea;
        [SerializeField] private Collider collision;
        [SerializeField] private uint addHp;
        [SerializeField] private float moveSpeed;
        [SerializeField] private int minWorkerCount;
        [SerializeField] private int maxWorkerCount;

        private PlayerController playerController;
        private RuntimeNavMeshBaker runtimeNavMeshBaker;
        private PlayerStatus playerStatus;

        public override void Initialize(IObjectResolver container)
        {
            playerController = container.Resolve<PlayerController>();
            runtimeNavMeshBaker = container.Resolve<RuntimeNavMeshBaker>();
            playerStatus = container.Resolve<PlayerStatus>();

            ForceComplete();

            assignableArea.OnWorkerEnter += async _ =>
            {
                if (WorkerCount > maxWorkerCount)
                    navMeshAgent.enabled = false;
                else if (WorkerCount >= minWorkerCount)
                {
                    collision.gameObject.layer = LayerMask.NameToLayer("Detection");
                    await runtimeNavMeshBaker.Bake();
                    navMeshAgent.enabled = true;
                }
            };

            assignableArea.OnWorkerExit += _ =>
            {
                if (WorkerCount < minWorkerCount)
                    navMeshAgent.enabled = false;
            };

            AdjustNavMesh().Forget();
        }

        private async UniTaskVoid AdjustNavMesh()
        {
            //手動更新のため無効化
            navMeshAgent.updateRotation = false;
            navMeshAgent.updatePosition = false;
            navMeshAgent.enabled = true;

            await UniTask.WaitUntil(() => navMeshAgent.isOnNavMesh, cancellationToken: this.GetCancellationTokenOnDestroy());
            await runtimeNavMeshBaker.Bake();

            navMeshAgent.enabled = false;
        }

        private void Update()
        {
            if (navMeshAgent.enabled && WorkerCount >= minWorkerCount)
            {
                navMeshAgent.SetDestination(playerController.transform.position);

                //対数関数的な変化に調整
                float speed = Mathf.Log10(WorkerCount - minWorkerCount + 2) * moveSpeed * Time.deltaTime;
                transform.position = Vector3.Lerp(transform.position, navMeshAgent.nextPosition, speed);
            }
        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                playerStatus.AddHp(addHp);
                ForceComplete();
                Disable();
            }
        }
    }
}