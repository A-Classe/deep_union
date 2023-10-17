using System;
using System.Collections.Generic;
using System.Threading;
using Core.Utility;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Module.Assignment;
using Module.Task;
using Module.Working;
using UnityEngine;
using UnityEngine.VFX;
using VContainer;
using Random = UnityEngine.Random;

namespace GameMain.Task
{
    public class PoisonArea : MonoBehaviour, IInjectable
    {
        [SerializeField] private uint poisonDamage = 5;
        [SerializeField] private float playerDamageInterval = 1f;
        [SerializeField] private float workerDamageInterval = 1f;
        [SerializeField] private float disappearDuration = 1f;
        [SerializeField] private float moveOffset = 1f;
        [Space] [SerializeField] private PoisonCreatureTask[] poisonCreatureTasks;
        [SerializeField] private VisualEffect toxicEffect1;
        [SerializeField] private VisualEffect toxicEffect2;
        [SerializeField] private Transform poisonWaterArea;
        [SerializeField] private ParticleSystem smokeEffect;

        private int killedCount;
        private PlayerStatus playerStatus;

        private List<Worker> workers = new List<Worker>();
        private CancellationTokenSource cTokenSource;
        private bool isPlayerEnter;

        [Inject]
        public void Construct(PlayerStatus playerStatus)
        {
            this.playerStatus = playerStatus;
            cTokenSource = new CancellationTokenSource();

            foreach (PoisonCreatureTask creatureTask in poisonCreatureTasks)
            {
                creatureTask.OnCompleted += OnPoisonCreatureKilled;
            }

            PlayerDamageLoop(cTokenSource.Token).Forget();
            WorkerDamageLoop(cTokenSource.Token).Forget();
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

        private void OnPoisonCreatureKilled(BaseTask _)
        {
            killedCount++;

            if (killedCount >= poisonCreatureTasks.Length)
            {
                poisonWaterArea.DOMoveZ(moveOffset, disappearDuration).Play();
                poisonWaterArea.DOScaleY(0f, disappearDuration)
                    .Play()
                    .OnComplete(() =>
                    {
                        gameObject.SetActive(false);
                    });

                toxicEffect1.Stop();
                toxicEffect2.Stop();
                smokeEffect.Stop();

                cTokenSource.Cancel();
                cTokenSource.Dispose();
                cTokenSource = null;
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

        private void OnDestroy()
        {
            cTokenSource?.Cancel();
            cTokenSource?.Dispose();
        }
    }
}