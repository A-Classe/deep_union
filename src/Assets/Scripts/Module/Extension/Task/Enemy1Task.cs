using System;
using System.Collections.Generic;
using System.Linq;
using Core.NavMesh;
using Core.Utility;
using Cysharp.Threading.Tasks;
using Module.Assignment.Component;
using Module.Extension.System;
using Module.Player;
using Module.Player.Controller;
using Module.Task;
using Module.Working;
using UnityEngine;
using VContainer;
using Random = UnityEngine.Random;

namespace Module.Extension.Task
{
    public class Enemy1Task : BaseTask
    {
        [Header("爆破ダメージ")] [SerializeField] private uint attackPoint;
        [Header("爆破するまでの時間")] [SerializeField] private float explodeLimit;
        [SerializeField] private uint attackPointToOtherTask;
        [SerializeField] private int workerDamageCount;
        [SerializeField] private LayerMask damageLayer;
        [SerializeField] private float damageRangeFixOffset;
        [SerializeField] private bool showExplodeRange;

        [SerializeField] private int dropCountMin;
        [SerializeField] private int dropCountMax;
        [SerializeField] private float dropSpreadForce;

        [SerializeField] private SimpleAgent simpleAgent;
        [SerializeField] private AssignableArea assignableArea;
        [SerializeField] private Transform explodeEffectSphere;
        [SerializeField] private Enemy1TaskDirector director;

        private Vector3 adsorptionOffset;
        private Transform adsorptionTarget;
        private List<Collider> damageBufferList;

        private bool isAdsorption;
        private RuntimeNavMeshBaker navMeshBaker;
        private HealTaskPool healTaskPool;
        private PlayerController playerController;
        private PlayerStatus playerStatus;

        private Transform playerTarget;
        private Collider[] workerDamageBuffer;

        public event Action OnBomb;

        private float progressTime;
        private bool IsMoving;

        public override void Initialize(IObjectResolver container)
        {
            playerController = container.Resolve<PlayerController>();
            playerStatus = container.Resolve<PlayerStatus>();
            navMeshBaker = container.Resolve<RuntimeNavMeshBaker>();
            healTaskPool = container.Resolve<HealTaskPool>();
            workerDamageBuffer = new Collider[128];
            damageBufferList = new List<Collider>();

            simpleAgent.SetActive(false);

            OnProgressChanged += director.UpdateScale;
            OnProgressChanged += director.UpdateBlinkColor;

            progressTime = 0.0f;
            IsMoving = false;
        }

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

            if (IsMoving)
            {
                progressTime += Time.deltaTime;
                var ratio = progressTime / explodeLimit;

                director.UpdateScale(ratio);
                director.UpdateBlinkColor(ratio);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (isAdsorption)
            {
                return;
            }

            if (other.gameObject.CompareTag("Player"))
            {
                Explode(other.transform);
            }
        }

        private async UniTaskVoid CountdownExplode()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(explodeLimit));

            if (isAdsorption)
            {
                return;
            }

            Explode(transform);
        }

        private void Explode(Transform target)
        {
            adsorptionTarget = target;
            adsorptionOffset = transform.position - target.position;
            isAdsorption = true;
            simpleAgent.SetActive(false);
            assignableArea.enabled = false;

            SetDetection(false);
            ExplodeSequence().Forget();
        }


        protected override async void OnComplete()
        {
            if (isAdsorption)
            {
                return;
            }

            isAdsorption = true;
            SetDetection(false);
            simpleAgent.SetActive(false);
            assignableArea.enabled = false;

            await director.DeadAnimation();

            Drop();

            //完了したら削除
            ForceComplete();
            Disable();
        }

        private void Drop()
        {
            Span<HealTask> tasks = healTaskPool.Get(Random.Range(dropCountMin, dropCountMax + 1));

            foreach (HealTask task in tasks)
            {
                task.transform.position = transform.position;
                task.Spread(dropSpreadForce);
            }
        }

        private async UniTaskVoid ExplodeSequence()
        {
            await director.AnimateExplode();

            //爆破エフェクト開始
            OnBomb?.Invoke();

            await director.EffectExplode();

            DetectExplosionArea();
            Damage();

            ForceComplete();
            Disable();
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
                    {
                        continue;
                    }

                    worker.Kill();
                    workerDamagedCount++;
                }

                //Damage to other tasks
                if (obj.parent != null && obj.parent.TryGetComponent(out BaseTask baseTask))
                {
                    if (baseTask == this || !baseTask.AcceptAttacks)
                    {
                        continue;
                    }

                    baseTask.ForceWork((int)attackPointToOtherTask);
                }
            }

            damageBufferList.Clear();
        }


        public void ForceEnable()
        {
            director.EnableMovingState();
            simpleAgent.SetActive(true);
            navMeshBaker?.Bake().Forget();

            IsMoving = true;

            CountdownExplode().Forget();
        }

        private void OnDrawGizmos()
        {
            if (!showExplodeRange)
            {
                return;
            }

            var origin = explodeEffectSphere.position;
            var radius = transform.localScale.x * explodeEffectSphere.localScale.x * damageRangeFixOffset;
            Gizmos.color = new Color(1f, 0f, 0f, 0.4f);
            Gizmos.DrawSphere(origin, radius);
        }
    }
}