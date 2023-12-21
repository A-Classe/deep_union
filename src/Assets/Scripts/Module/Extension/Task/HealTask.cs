using System;
using Cysharp.Threading.Tasks;
using Module.Assignment.Component;
using Module.Player;
using Module.Player.Controller;
using Module.Task;
using UnityEngine;
using UnityEngine.AI;
using VContainer;
using Random = UnityEngine.Random;

namespace Module.Extension.Task
{
    public class HealTask : BaseTask
    {
        [SerializeField] private NavMeshAgent navMeshAgent;
        [SerializeField] private AssignableArea assignableArea;
        [SerializeField] private Rigidbody rig;
        [SerializeField] private Collider collision;
        [SerializeField] private uint addHp;
        [SerializeField] private float moveSpeed;
        [SerializeField] private int minWorkerCount;
        [SerializeField] private int maxWorkerCount;

        [SerializeField] private GameObject collideObj;
        [SerializeField] private GameObject healObj;

        private PlayerController playerController;
        private PlayerStatus playerStatus;
        private TaskActivator taskActivator;

        public event Action<HealTask> OnCollected;

        private void Update()
        {
            if (navMeshAgent.enabled && WorkerCount >= minWorkerCount)
            {
                navMeshAgent.SetDestination(playerController.transform.position);

                //対数関数的な変化に調整
                var workerCount = Mathf.Min(WorkerCount, maxWorkerCount);
                var speed = Mathf.Log10(workerCount - minWorkerCount + 2) * moveSpeed * Time.deltaTime;
                transform.position = Vector3.Lerp(transform.position, navMeshAgent.nextPosition, speed);
            }
        }

        private async void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                playerStatus.AddHp(addHp);
                ForceComplete();

                collideObj.SetActive(false);
                healObj.SetActive(false);

                await WaitSound();

                OnCollected?.Invoke(this);
                taskActivator.ForceDeactivate(this);
            }
        }

        public override void Initialize(IObjectResolver container)
        {
            collision.gameObject.layer = LayerMask.NameToLayer("Detection");
            playerController = container.Resolve<PlayerController>();
            playerStatus = container.Resolve<PlayerStatus>();
            taskActivator = container.Resolve<TaskActivator>();

            ForceComplete();

            assignableArea.OnWorkerEnter += (_, _) =>
            {
                if (WorkerCount >= minWorkerCount)
                {
                    navMeshAgent.enabled = true;
                }
            };

            assignableArea.OnWorkerExit += (_, _) =>
            {
                if (WorkerCount < minWorkerCount)
                {
                    navMeshAgent.enabled = false;
                }
            };

            navMeshAgent.updatePosition = false;
            navMeshAgent.updateRotation = false;
        }

        //仮でヒール音を待機
        private async UniTask WaitSound()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(1f));
        }

        public void Spread(float force)
        {
            Vector2 forceDir = Random.insideUnitCircle * force;
            rig.AddForce(forceDir.x, 0f, forceDir.y);
        }
    }
}