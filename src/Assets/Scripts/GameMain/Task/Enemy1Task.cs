using System;
using System.Collections.Generic;
using System.Linq;
using Core.NavMesh;
using Core.Utility;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using GameMain.System;
using Module.Assignment;
using Module.Assignment.Component;
using Module.Player;
using Module.Player.Controller;
using Module.Task;
using Module.Working;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using VContainer;
using Wanna.DebugEx;
using Random = UnityEngine.Random;

namespace GameMain.Task
{
    public class Enemy1Task : BaseTask
    {
        [Header("爆破ダメージ")] [SerializeField] private uint attackPoint;
        [SerializeField] private uint attackPointToOtherTask;
        [SerializeField] private int workerDamageCount;
        [SerializeField] private float explodeStartWaitTime;
        [SerializeField] private float disableBodyDelay;
        [SerializeField] private LayerMask damageLayer;
        [SerializeField] private float damageRangeFixOffset;
        [SerializeField] private bool showExplodeRange;

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
        [SerializeField] private Transform explodeEffectSphere;
        [SerializeField] private Renderer bodyRenderer;
        [SerializeField] private BombEffectEvent bombEffectEvent;

        private Transform playerTarget;
        private PlayerController playerController;
        private PlayerStatus playerStatus;
        private RuntimeNavMeshBaker navMeshBaker;
        private Material bodyMaterial;
        private Collider[] workerDamageBuffer;
        private List<Collider> damageBufferList;
        [SerializeField] private Color blinkColor;

        private bool isAdsorption;
        private Transform adsorptionTarget;
        private Tween blinkTween;
        private Vector3 adsorptionOffset;
        private Vector3 initialScale;
        private static readonly int IsWalking = Animator.StringToHash("IsWalking");
        private static readonly int CanBomb = Animator.StringToHash("CanBomb");
        private static readonly int BodyColor = Shader.PropertyToID("_Color");

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
            {
                blinkTween = bodyMaterial.DOColor(blinkColor, BodyColor, 1f).SetLoops(-1, LoopType.Yoyo).Play();
            }

            blinkTween.timeScale = damageBlinkCurve.Evaluate(progress);
        }

        private void UpdateScale(float progress)
        {
            scaleBody.localScale = initialScale * damageScaleCurve.Evaluate(progress);
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
            base.Disable();
        }

        private async UniTaskVoid DisableBody()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(disableBodyDelay));

            animator.gameObject.SetActive(false);
        }

        private void DetectExplosionArea()
        {
            //周囲のコライダーを取得
            Vector3 origin = explodeEffectSphere.position;
            float radius = transform.localScale.x * explodeEffectSphere.localScale.x * damageRangeFixOffset;
            int count = Physics.OverlapSphereNonAlloc(origin, radius, workerDamageBuffer, damageLayer);

            damageBufferList.AddRange(workerDamageBuffer.Take(count));
        }

        private void Damage()
        {
            int workerDamagedCount = 0;

            foreach (Transform obj in damageBufferList.Shuffle().Select(item => item.transform))
            {
                //Damage to player
                if (obj.CompareTag("Player"))
                {
                    playerStatus.RemoveHp(attackPoint);
                    continue;
                }

                //Damage to worker
                if (workerDamagedCount < workerDamageCount && obj.parent != null && obj.parent.TryGetComponent(out Worker worker))
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

        public override void Enable() { }

        public override void Disable() { }

        private void OnDrawGizmos()
        {
            if (!showExplodeRange)
                return;

            Vector3 origin = explodeEffectSphere.position;
            float radius = transform.localScale.x * explodeEffectSphere.localScale.x * damageRangeFixOffset;
            Gizmos.color = new Color(1f, 0f, 0f, 0.4f);
            Gizmos.DrawSphere(origin, radius);
        }
    }
}