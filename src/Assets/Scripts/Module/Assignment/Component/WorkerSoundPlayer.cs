using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Wanna.DebugEx;

namespace Module.Working
{
    [RequireComponent(typeof(AudioSource))]
    [DisallowMultipleComponent]
    public class WorkerSoundPlayer : MonoBehaviour
    {
        private AudioSource audioSource;
        [SerializeField] private AudioClip assignSound;
        [SerializeField] private float minPlayInterval;
        [SerializeField] private float maxQueueCount;

        private readonly Queue<AudioClip> clipQueue = new(64);

        private void Start()
        {
            audioSource = GetComponent<AudioSource>();

            if (audioSource == null)
            {
                DebugEx.LogError($"{gameObject.name}にAudioSourceがアタッチされていません");
            }

            PlayLoop().Forget();
        }

        private async UniTaskVoid PlayLoop()
        {
            CancellationToken canceller = this.GetCancellationTokenOnDestroy();

            while (!canceller.IsCancellationRequested)
            {
                //なにかスケジュールされるまで待機
                await UniTask.WaitUntil(() => clipQueue.Count > 0, cancellationToken: canceller);

                audioSource.PlayOneShot(clipQueue.Dequeue());

                //次の再生まで待機
                await UniTask.Delay(TimeSpan.FromSeconds(minPlayInterval), cancellationToken: canceller);
            }
        }

        public void PlayOnAssign()
        {
            PlayCore(assignSound);
        }

        public void PlayOnRelease()
        {
            //リリース用に変える
            PlayCore(assignSound);
        }

        private void PlayCore(AudioClip audioClip)
        {
            //最大待機量を超えたら再生しない
            if (clipQueue.Count >= maxQueueCount)
            {
                return;
            }

            //再生中じゃないときはそのまま再生
            if (!audioSource.isPlaying)
            {
                audioSource.PlayOneShot(audioClip);
                return;
            }

            //スケジュールする
            clipQueue.Enqueue(audioClip);
        }
    }
}