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
        [SerializeField] private float deathDuration;
        [SerializeField] private Renderer[] cutOffRenderers;
        private List<Material> cutOffMaterials;
        private int cutOffId = Shader.PropertyToID("_CutOffHeight");

        private IWorkerState currentState;
        private IWorkerState[] workerStates;
        private NavMeshAgent navMeshAgent;

        public Animator animator;
        public Transform AreaTarget { get; private set; }
        public Transform Target { get; private set; }

        public bool IsLocked { get; private set; }
        public bool IsWorldMoving { get; set; }
        public Action<Worker> OnDead { get; set; }

        private void Awake()
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
            IEnumerable<Material[]> materials = cutOffRenderers.Select(renderer => renderer.materials);

            foreach (Material[] rendMaterial in materials)
            {
                foreach (Material material in rendMaterial)
                {
                    cutOffMaterials.Add(material);
                }
            }
        }

        private void Update()
        {
            currentState?.Update();
        }

        /// <summary>
        /// ワーカーの状態をセットします
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
            gameObject.SetActive(true);
        }

        public async UniTaskVoid Disable()
        {
            await DeathCutoff(this.GetCancellationTokenOnDestroy());

            gameObject.SetActive(false);
            SetWorkerState(WorkerState.Idle);
            OnDead = null;
        }

        private async UniTask DeathCutoff(CancellationToken cancellationToken)
        {
            float currentValue = 0f;
            float currentTime = 0f;

            while (!cancellationToken.IsCancellationRequested)
            {
                currentValue = Mathf.Lerp(0f, 12f, Mathf.InverseLerp(0f, deathDuration, currentTime));

                foreach (Material material in cutOffMaterials)
                {
                    material.SetFloat(cutOffId, currentValue);
                }

                if (currentTime > deathDuration)
                    return;

                currentTime += Time.fixedDeltaTime;

                await UniTask.Yield(PlayerLoopTiming.FixedUpdate, cancellationToken);
            }
        }

        public void Dispose()
        {
            foreach (IWorkerState state in workerStates)
            {
                state.Dispose();
            }

            Destroy(gameObject);
        }

        private void OnDestroy()
        {
            foreach (IWorkerState state in workerStates)
            {
                state.Dispose();
            }
        }
    }
}