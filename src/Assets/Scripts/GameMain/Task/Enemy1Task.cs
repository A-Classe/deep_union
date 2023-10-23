using System;
using Core.NavMesh;
using Cysharp.Threading.Tasks;
using GameMain.System;
using Module.Assignment;
using Module.Assignment.Component;
using Module.Player.Controller;
using Module.Task;
using UnityEngine;
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
        private RuntimeNavMeshBaker navMeshBaker;

        private bool isAdsorption;
        private Transform adsorptionTarget;
        private Vector3 adsorptionOffset;


        public override void Initialize(IObjectResolver container)
        {
            playerController = container.Resolve<PlayerController>();
            playerStatus = container.Resolve<PlayerStatus>();
            navMeshBaker = container.Resolve<RuntimeNavMeshBaker>();
            decalProjector.enabled = false;
            simpleAgent.SetActive(false);

            SetDetection(false);
            base.Disable();
        }

        private void OnEnable()
        {
            decalProjector.enabled = true;
            simpleAgent.SetActive(true);

            navMeshBaker?.Bake().Forget();
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
            base.Disable();
            assignableArea.enabled = false;
        }

        private async UniTaskVoid ExplodeSequence()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(explodeDuration));

            playerStatus.RemoveHp(attackPoint);

            ForceComplete();
            base.Disable();
        }


        public void ForceEnable()
        {
            gameObject.SetActive(true);
        }

        public override void Enable() { }

        public override void Disable() { }
    }
}