using System;
using System.Collections.Generic;
using System.Linq;
using Core.NavMesh;
using Core.Utility;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Module.Assignment.Component;
using Module.Extension.System;
using Module.Player;
using Module.Player.Controller;
using Module.Task;
using Module.Working;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using VContainer;

namespace Module.Extension.Task
{
    public class Enemy1Task : BaseTask
    {
        private static readonly int IsWalking = Animator.StringToHash("IsWalking");
        private static readonly int CanBomb = Animator.StringToHash("CanBomb");
        private static readonly int BodyColor = Shader.PropertyToID("_Color");
        [Header("爆破ダメージ")] [SerializeField] private uint attackPoint;
        [SerializeField] private uint attackPointToOtherTask;
        [SerializeField] private int workerDamageCount;
        [SerializeField] private float explodeStartWaitTime;
        [SerializeField] private float disableBodyDelay;
        [SerializeField] private LayerMask damageLayer;
        [SerializeField] private float damageRangeFixOffset;
        [SerializeField] private bool showExplodeRange;

        [Header("ダメージとスケールの比例関数")] [SerializeField]
        private AnimationCurve damageScaleCurve;

        [Header("ダメージと点滅の比例関数")] [SerializeField]
        private AnimationCurve damageBlinkCurve;

        [SerializeField] private SimpleAgent simpleAgent;
        [SerializeField] private DecalProjector decalProjector;
        [SerializeField] private AssignableArea assignableArea;
        [SerializeField] private Animator animator;
        [SerializeField] private Transform scaleBody;
        [SerializeField] private Transform explodeEffectSphere;
        [SerializeField] private Renderer bodyRenderer;
        [SerializeField] private BombEffectEvent bombEffectEvent;
        [SerializeField] private Color blinkColor;
        private Vector3 adsorptionOffset;
        private Transform adsorptionTarget;
        private Tween blinkTween;
        private Material bodyMaterial;
        private List<Collider> damageBufferList;
        private Vector3 initialScale;

        private bool isAdsorption;
        private RuntimeNavMeshBaker navMeshBaker;
        private PlayerController playerController;
        private PlayerStatus playerStatus;

        private Transform playerTarget;
        private Collider[] workerDamageBuffer;

        private void Update()
        {
            if (isAdsorption)
            {
                var adsorptionPosition = adsorptionTarget == null ? transform.position : adsorptionTarget.position;
                transform.position = adsorptionPosition + adsorptionOffset;
            }
            else
            {
                simpleAgent.Move(playerController.transform.position);
            }
        }

        private void OnDrawGizmos()
        {
            if (!showExplodeRange)
                return;

            var origin = explodeEffectSphere.position;
            var radius = transform.localScale.x * explodeEffectSphere.localScale.x * damageRangeFixOffset;
            Gizmos.color = new Color(1f, 0f, 0f, 0.4f);
            Gizmos.DrawSphere(origin, radius);
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

        public event Action OnBomb;

        public override void Initialize(IObjectResolver container)
        {
            playerController = container.Resolve<PlayerController>();
            playerStatus = container.Resolve<PlayerStatus>();
            navMeshBaker = container.Resolve<RuntimeNavMeshBaker>();
            bodyMaterial = bodyRenderer.material;
            workerDamageBuffer = new Collider[128];
            damageBufferList = new List<Collider>();
            initialScale = transform.localScale;

            decalProjector.enabled = false;
            simpleAgent.SetActive(false);

            OnProgressChanged += UpdateScale;
            OnProgressChanged += UpdateBlinkColor;
        }

        private void UpdateBlinkColor(float progress)
        {
            if (blinkTween == null)
                blinkTween = bodyMaterial.DOColor(blinkColor, BodyColor, 1f).SetLoops(-1, LoopType.Yoyo).Play();

            blinkTween.timeScale = damageBlinkCurve.Evaluate(progress);
        }

        private void UpdateScale(float progress)
        {
            scaleBody.localScale = initialScale * damageScaleCurve.Evaluate(progress);
        }

        protected override void OnComplete()
        {
            if (isAdsorption)
                return;

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
            OnBomb?.Invoke();
            await bombEffectEvent.Bomb();

            DetectExplosionArea();
            Damage();

            ForceComplete();
            Disable();
        }

        private async UniTaskVoid DisableBody()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(disableBodyDelay));

            animator.gameObject.SetActive(false);
        }

        private void DetectExplosionArea()
        {
            //周囲のコライダーを取得
            var origin = explodeEffectSphere.position;
            var radius = transform.localScale.x * explodeEffectSphere.localScale.x * damageRangeFixOffset;
            var count = Physics.OverlapSphereNonAlloc(origin, radius, workerDamageBuffer, damageLayer);

            damageBufferList.AddRange(workerDamageBuffer.Take(count));
        }

        private void Damage()
        {
            var workerDamagedCount = 0;

            foreach (var obj in damageBufferList.Shuffle().Select(item => item.transform))
            {
                //Damage to player
                if (obj.CompareTag("Player"))
                {
                    playerStatus.RemoveHp(attackPoint);
                    continue;
                }

                //Damage to worker
                if (workerDamagedCount < workerDamageCount && obj.TryGetComponent(out Worker worker))
                {
                    if (worker.IsLocked)
                        continue;

                    worker.Kill();
                    workerDamagedCount++;
                }

                //Damage to other tasks
                if (obj.parent != null && obj.parent.TryGetComponent(out BaseTask baseTask))
                {
                    if (baseTask == this || !baseTask.AcceptAttacks)
                        continue;

                    baseTask.ForceWork((int)attackPointToOtherTask);
                }
            }

            damageBufferList.Clear();
        }


        public void ForceEnable()
        {
            animator.SetBool(IsWalking, true);
            decalProjector.enabled = true;
            simpleAgent.SetActive(true);
            navMeshBaker?.Bake().Forget();
        }
    }
}