using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

namespace Module.Working
{
    public class WorkerRandomizer : MonoBehaviour
    {
        [Header("速度のランダム範囲")] [SerializeField] private RandomValueFloat speedRange;

        [Header("加速度のランダム範囲")]
        [SerializeField]
        private RandomValueFloat accelerationRange;

        [Header("高さのランダム範囲")] [SerializeField] private RandomValueFloat heightRange;

        [Header("速度変化インターバルのランダム範囲")]
        [SerializeField]
        private RandomValueFloat changeIntervalSpeedRange;

        [SerializeField] private NavMeshAgent navMeshAgent;
        [SerializeField] private Transform bodyTransform;

        private CancellationTokenSource activeCancellationTokenSource;

        public void SetUp()
        {
            navMeshAgent.speed = speedRange.MakeValue();
            navMeshAgent.acceleration = accelerationRange.MakeValue();

            float height = heightRange.MakeValue();
            Vector3 localPosition = bodyTransform.localPosition;
            localPosition.y = height;
            bodyTransform.localPosition = localPosition;
        }

        private async UniTask ChangeSpeedLoop()
        {
            CancellationToken destroyCanceller = this.GetCancellationTokenOnDestroy();

            while (!activeCancellationTokenSource.IsCancellationRequested && !destroyCanceller.IsCancellationRequested)
            {
                TimeSpan waitDuration = TimeSpan.FromSeconds(changeIntervalSpeedRange.MakeValue());
                await UniTask.Delay(waitDuration, cancellationToken: destroyCanceller);

                navMeshAgent.speed = speedRange.MakeValue();
                navMeshAgent.acceleration = accelerationRange.MakeValue();
            }
        }

        public void Enable()
        {
            activeCancellationTokenSource = new CancellationTokenSource();
            ChangeSpeedLoop().Forget();
        }


        public void Disable()
        {
            activeCancellationTokenSource?.Cancel();
            activeCancellationTokenSource?.Dispose();
        }
    }
}