using System;
using System.Collections.Generic;
using System.Threading;
using Core.Utility;
using Cysharp.Threading.Tasks;
using Module.Assignment;
using Module.Working;
using UnityEngine;
using VContainer;
using Wanna.DebugEx;
using Random = UnityEngine.Random;

namespace GameMain.Task
{
    public class PoisonArea : MonoBehaviour, IInjectable
    {
        [SerializeField] private uint poisonDamage = 5;
        [SerializeField] private float playerDamageInterval = 1f;
        [SerializeField] private float workerDamageInterval = 1f;
        private PlayerStatus playerStatus;
        private WorkerAgent workerAgent;

        private List<Worker> workers = new List<Worker>();
        private bool isPlayerEnter;

        [Inject]
        public void Construct(PlayerStatus playerStatus, WorkerAgent workerAgent)
        {
            this.playerStatus = playerStatus;
            this.workerAgent = workerAgent;

            PlayerDamageLoop(this.GetCancellationTokenOnDestroy()).Forget();
            WorkerDamageLoop(this.GetCancellationTokenOnDestroy()).Forget();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                isPlayerEnter = true;
            }
            else if (other.transform.parent.TryGetComponent(out Worker worker))
            {
                workers.Add(worker);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                isPlayerEnter = false;
            }
            else if (other.transform.parent.TryGetComponent(out Worker worker))
            {
                workers.Remove(worker);
            }
        }

        private async UniTaskVoid PlayerDamageLoop(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                if (isPlayerEnter)
                {
                    playerStatus.RemoveHp(poisonDamage);
                }

                await UniTask.Delay(TimeSpan.FromSeconds(playerDamageInterval), cancellationToken: cancellationToken);
            }
        }


        private async UniTaskVoid WorkerDamageLoop(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                if (workers.Count > 0)
                {
                    int index = Random.Range(0, workers.Count);
                    Worker worker = workers[index];
                    workers.RemoveAt(index);
                    worker.Kill();
                }

                await UniTask.Delay(TimeSpan.FromSeconds(workerDamageInterval), cancellationToken: cancellationToken);
            }
        }
    }
}