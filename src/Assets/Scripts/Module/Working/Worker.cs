using System;
using System.Linq;
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
        private IWorkerState currentState;
        private IWorkerState[] workerStates;
        private NavMeshAgent navMeshAgent;

        public Animator animator;
        public Transform Target { get; private set; }

        public bool IsLocked { get; private set; }
        public bool IsWorldMoving { get; set; }

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

        public void SetFollowTarget(Transform target)
        {
            Target = target;
        }

        public void SetLockState(bool isLocked)
        {
            IsLocked = isLocked;
            navMeshAgent.enabled = !isLocked;
        }

        public void Enable()
        {
            gameObject.SetActive(true);
        }

        public void Disable()
        {
            gameObject.SetActive(true);
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