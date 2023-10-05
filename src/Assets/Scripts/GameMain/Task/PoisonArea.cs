using System;
using System.Threading;
using Core.Utility;
using Cysharp.Threading.Tasks;
using Module.Assignment;
using UnityEngine;
using VContainer;

namespace GameMain.Task
{
    public class PoisonArea : MonoBehaviour, IInjectable
    {
        [SerializeField] private uint poisonDamage = 5;
        [SerializeField] private float damageInterval = 2f;
        private PlayerStatus playerStatus;

        private CancellationTokenSource cTokenSource;

        [Inject]
        public void Construct(PlayerStatus playerStatus)
        {
            this.playerStatus = playerStatus;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                cTokenSource = new CancellationTokenSource();
                DamageLoop(cTokenSource.Token).Forget();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                cTokenSource?.Cancel();
                cTokenSource?.Dispose();
            }
        }

        private async UniTaskVoid DamageLoop(CancellationToken cancellationToken)
        {
            while (playerStatus.Hp > 0 && !cancellationToken.IsCancellationRequested)
            {
                playerStatus.RemoveHp(poisonDamage);

                await UniTask.Delay(TimeSpan.FromSeconds(damageInterval), cancellationToken: cancellationToken);
            }
        }
    }
}