using System;
using Cysharp.Threading.Tasks;
using GameMain.System;
using Module.Assignment;
using Module.Assignment.Component;
using Module.Player.Controller;
using Module.Task;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering.Universal;
using VContainer;
using Wanna.DebugEx;

namespace GameMain.Task
{
    public class Enemy1Task : BaseTask
    {
        [SerializeField] private uint attackPoint;
        [SerializeField] private float explodeDuration;
        [SerializeField] private SimpleAgent simpleAgent;
        [SerializeField] private DecalProjector decalProjector;
        [SerializeField] private AssignableArea assignableArea;

        private Transform playerTarget;
        private PlayerController playerController;
        private PlayerStatus playerStatus;

        private bool isAdsorption;
        private Transform adsorptionTarget;
        private Vector3 adsorptionOffset;


        public override void Initialize(IObjectResolver container)
        {
            playerController = container.Resolve<PlayerController>();
            playerStatus = container.Resolve<PlayerStatus>();
            decalProjector.enabled = false;
            simpleAgent.SetActive(false);

            SetDetection(false);
            Disable();
        }

        protected override void ManagedUpdate(float deltaTime) { }

        private void OnEnable()
        {
            decalProjector.enabled = true;
            simpleAgent.SetActive(true);
        }

        private void Update()
        {
            if (isAdsorption)
            {
                transform.position = adsorptionTarget.position + adsorptionOffset;
            }
            else
            {
                simpleAgent.Move(playerController.transform.position);
            }
        }

        private void OnCollisionEnter(Collision other)
        {
            if (isAdsorption)
                return;

            if (other.gameObject.CompareTag("Player"))
            {
                adsorptionTarget = other.transform;
                adsorptionOffset = transform.position - adsorptionTarget.position;
                isAdsorption = true;
                simpleAgent.SetActive(false);
                assignableArea.enabled = false;

                SetDetection(false);
                ExplodeSequence().Forget();
            }
        }

        protected override void OnComplete()
        {
            isAdsorption = true;
            SetDetection(false);
            Disable();
            assignableArea.enabled = false;
        }

        private async UniTaskVoid ExplodeSequence()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(explodeDuration));

            playerStatus.RemoveHp(attackPoint);

            ForceComplete();
            Disable();
        }
    }
}