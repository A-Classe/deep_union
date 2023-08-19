using System;
using System.Linq;
using Module.Worker.State;
using UnityEngine;
using UnityEngine.AI;
using Wanna.DebugEx;

namespace Module.Worker
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class Worker : MonoBehaviour
    {
        public Vector3 FollowPoint { get; private set; }

        private Vector3 offset;
        private IWorkerState[] workerStates;
        private IWorkerState currentState;

        private void Awake()
        {
            workerStates = new IWorkerState[]
            {
                new IdleState(this),
                new FollowState(this),
                new WorkState(this),
            };

            SetWorkerState(WorkerState.Idle);
        }

        private void Update()
        {
            currentState?.Update();
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

        public void OnSpawn(Vector3 spawnPoint)
        {
            offset = transform.position - spawnPoint;
        }

        public void SetFollowPoint(Vector3 point)
        {
            FollowPoint = point + offset;
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