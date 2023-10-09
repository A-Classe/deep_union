using System;
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
        [SerializeField] private uint addHp;
        [SerializeField] private int minWorkerCount;
        [SerializeField] private int maxWorkerCount;

        private PlayerController playerController;
        private PlayerStatus playerStatus;

        public override void Initialize(IObjectResolver container)
        {
            playerController = container.Resolve<PlayerController>();
            playerStatus = container.Resolve<PlayerStatus>();

            ForceComplete();

            assignableArea.OnWorkerEnter += _ =>
            {
                if (WorkerCount > maxWorkerCount)
                    navMeshAgent.enabled = false;
                else if (WorkerCount >= minWorkerCount)
                    navMeshAgent.enabled = true;
            };

            assignableArea.OnWorkerExit += _ =>
            {
                if (WorkerCount < minWorkerCount)
                    navMeshAgent.enabled = false;
            };

            navMeshAgent.updateRotation = false;
        }

        private void Update()
        {
            if (navMeshAgent.enabled)
            {
                navMeshAgent.SetDestination(playerController.transform.position);
            }
        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                playerStatus.AddHp(addHp);
                Disable();
            }
        }
    }
}