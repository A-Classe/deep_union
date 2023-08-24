using System;
using System.Linq;
using Module.Working.State;
using UnityEngine;
using UnityEngine.AI;
using Wanna.DebugEx;

namespace Module.Working
{
    /// <summary>
    /// ワーカーの状態を管理するクラス
    /// </summary>
    [RequireComponent(typeof(NavMeshAgent))]
    public class Worker : MonoBehaviour
    {
        private IWorkerState[] workerStates;
        private IWorkerState currentState;

        public Transform Target { get; private set; }

        private void Update()
        {
            currentState?.Update();
        }

        public void OnSpawn(Vector3 spawnPos)
        {
            workerStates = new IWorkerState[]
            {
                new IdleState(this),
                new FollowState(this, spawnPos),
                new WorkState(this),
            };

            SetWorkerState(WorkerState.Idle);
        }

        public void SetWorkerState(WorkerState workerState)
        {
            try
            {
                currentState = workerStates.First(state => state.WorkerState == workerState);
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
            this.Target = target;
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
            Destroy(gameObject);
        }
    }
}