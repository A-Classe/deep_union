using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Module.Working.State;
using UnityEngine;
using UnityEngine.AI;
using Wanna.DebugEx;

namespace Module.Working
{
    /// <summary>
    ///     ワーカーの状態を管理するクラス
    /// </summary>
    [RequireComponent(typeof(NavMeshAgent))]
    public class Worker : MonoBehaviour
    {
        [SerializeField] private bool initialized;
        [SerializeField] private float deathDuration;
        [SerializeField] private Renderer[] cutOffRenderers;
        [SerializeField] private WorkerRandomizer workerRandomizer;

        public Animator animator;

        private IWorkerState currentState;
        private readonly int cutOffId = Shader.PropertyToID("_CutOffHeight");
        private readonly int deadId = Animator.StringToHash("Dead");
        private List<Material> cutOffMaterials;
        private NavMeshAgent navMeshAgent;
        private IWorkerState[] workerStates;
        public Transform AreaTarget { get; private set; }
        public Transform Target { get; private set; }

        public bool IsLocked { get; private set; }
        public bool IsWorldMoving { get; set; }
        public Action<Worker> OnDead { get; set; }

        private void Update()
        {
            if (!initialized)
                return;

            if (navMeshAgent.pathStatus != NavMeshPathStatus.PathInvalid) currentState?.Update();
        }

        private void OnDestroy()
        {
            foreach (var state in workerStates) state.Dispose();
        }

        public async UniTaskVoid Initialize()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            workerStates = new IWorkerState[]
            {
                new IdleState(this),
                new FollowState(this),
                new WorkState(this)
            };


            SetWorkerState(WorkerState.Idle);

            cutOffMaterials = new List<Material>();
            var materials = cutOffRenderers.Select(renderer => renderer.materials);

            foreach (var rendMaterial in materials)
            {
                foreach (var material in rendMaterial)
                    cutOffMaterials.Add(material);
            }

            await SetUpNavMesh();

            initialized = true;
        }

        private async UniTask SetUpNavMesh()
        {
            workerRandomizer.SetUp();
            navMeshAgent.enabled = true;

            await UniTask.WaitUntil(() => navMeshAgent.isOnNavMesh,
                cancellationToken: this.GetCancellationTokenOnDestroy());
        }

        /// <summary>
        ///     ワーカーの状態をセットします
        /// </summary>
        /// <param name="workerState">セットするWorkerState</param>
        public void SetWorkerState(WorkerState workerState)
        {
            try
            {
                currentState?.OnStop();
                currentState = workerStates.First(state => state.WorkerState == workerState);
                currentState.OnStart();
            }
            catch (Exception e)
            {
                DebugEx.LogError("存在しないステートをセットしました");
                DebugEx.LogException(e);
                throw;
            }
        }

        public void SetFollowTarget(Transform areaTarget, Transform target)
        {
            AreaTarget = areaTarget;
            Target = target;
        }

        public void SetLockState(bool isLocked)
        {
            IsLocked = isLocked;
            navMeshAgent.enabled = !isLocked;
        }

        public void Kill()
        {
            OnDead?.Invoke(this);
        }

        public void Enable()
        {
            SetLockState(false);
            gameObject.SetActive(true);
            workerRandomizer.Enable();
        }

        public async UniTaskVoid Disable()
        {
            workerRandomizer.Disable();
            SetLockState(true);

            animator.SetBool(deadId, true);

            await DeathCutoff(this.GetCancellationTokenOnDestroy());

            gameObject.SetActive(false);
            SetWorkerState(WorkerState.Idle);
            OnDead = null;
        }

        private async UniTask DeathCutoff(CancellationToken cancellationToken)
        {
            var currentValue = 0f;
            var currentTime = 0f;

            while (!cancellationToken.IsCancellationRequested)
            {
                currentValue = Mathf.Lerp(-1f, 2f, Mathf.InverseLerp(0f, deathDuration, currentTime));

                foreach (var material in cutOffMaterials) material.SetFloat(cutOffId, currentValue);

                if (currentTime > deathDuration)
                    return;

                currentTime += Time.fixedDeltaTime;

                await UniTask.Yield(PlayerLoopTiming.FixedUpdate, cancellationToken);
            }
        }

        public void SetAvoidPlayer(bool enabled)
        {
            // navMeshAgent.
        }

        public void Teleport(Vector3 position)
        {
            navMeshAgent.Warp(position);
        }

        public void Dispose()
        {
            foreach (var state in workerStates) state.Dispose();

            Destroy(gameObject);
        }
    }
}