using System;
using Core.NavMesh;
using Cysharp.Threading.Tasks;
using DG.Tweening;
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
        [Header("爆破ダメージ")] [SerializeField] private uint attackPoint;
        [SerializeField] private float explodeStartWaitTime;
        [SerializeField] private float disableBodyDelay;

        [Header("ダメージとスケールの比例関数")]
        [SerializeField]
        private AnimationCurve damageScaleCurve;

        [Header("ダメージと点滅の比例関数")]
        [SerializeField]
        private AnimationCurve damageBlinkCurve;

        [SerializeField] private SimpleAgent simpleAgent;
        [SerializeField] private DecalProjector decalProjector;
        [SerializeField] private AssignableArea assignableArea;
        [SerializeField] private Animator animator;
        [SerializeField] private Transform scaleBody;
        [SerializeField] private Renderer bodyRenderer;
        [SerializeField] private BombEffectEvent bombEffectEvent;

        private Transform playerTarget;
        private PlayerController playerController;
        private PlayerStatus playerStatus;
        private RuntimeNavMeshBaker navMeshBaker;
        private Material bodyMaterial;
        [SerializeField] private Color blinkColor;

        private bool isAdsorption;
        private Transform adsorptionTarget;
        private Tween blinkTween;
        private Vector3 adsorptionOffset;
        private static readonly int IsWalking = Animator.StringToHash("IsWalking");
        private static readonly int CanBomb = Animator.StringToHash("CanBomb");
        private static readonly int BodyColor = Shader.PropertyToID("_Color");


        public override void Initialize(IObjectResolver container)
        {
            playerController = container.Resolve<PlayerController>();
            playerStatus = container.Resolve<PlayerStatus>();
            navMeshBaker = container.Resolve<RuntimeNavMeshBaker>();
            bodyMaterial = bodyRenderer.material;

            decalProjector.enabled = false;
            simpleAgent.SetActive(false);

            SetDetection(false);
            base.Disable();

            OnProgressChanged += UpdateScale;
            OnProgressChanged += UpdateBlinkColor;
        }

        private void UpdateBlinkColor(float progress)
        {
            if (blinkTween == null)
            {
                blinkTween = bodyMaterial.DOColor(blinkColor, BodyColor, 1f).SetLoops(-1, LoopType.Yoyo).Play();
            }

            blinkTween.timeScale = damageBlinkCurve.Evaluate(progress);
        }

        private void UpdateScale(float progress)
        {
            scaleBody.localScale = Vector3.one * damageScaleCurve.Evaluate(progress);
        }

        private void OnEnable()
        {
            decalProjector.enabled = true;
            simpleAgent.SetActive(true);

            animator.SetBool(IsWalking, true);

            navMeshBaker?.Bake().Forget();
        }

        private void Update()
        {
            if (isAdsorption)
            {
                Vector3 adsorptionPosition = adsorptionTarget == null ? transform.position : adsorptionTarget.position;
                transform.position = adsorptionPosition + adsorptionOffset;
            }
            else
            {
                simpleAgent.Move(playerController.transform.position);
            }
        }

        private void OnTriggerEnter(Collider other)
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
            assignableArea.enabled = false;

            ExplodeSequence().Forget();
        }

        private async UniTaskVoid ExplodeSequence()
        {
            //爆破モーション開始
            animator.SetBool(IsWalking, false);
            animator.SetTrigger(CanBomb);
            await UniTask.Delay(TimeSpan.FromSeconds(explodeStartWaitTime));

            DisableBody().Forget();

            //爆破エフェクト開始
            await bombEffectEvent.Bomb();

            playerStatus.RemoveHp(attackPoint);

            ForceComplete();
            base.Disable();
        }

        private async UniTaskVoid DisableBody()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(disableBodyDelay));

            animator.gameObject.SetActive(false);
        }


        public void ForceEnable()
        {
            gameObject.SetActive(true);
        }

        public override void Enable() { }

        public override void Disable() { }
    }
}