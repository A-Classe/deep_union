using System;
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
        private int scheduledCount;

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
                await UniTask.WaitUntil(() => scheduledCount > 0, cancellationToken: canceller);

                audioSource.PlayOneShot(assignSound);
                scheduledCount--;

                //次の再生まで待機
                await UniTask.Delay(TimeSpan.FromSeconds(minPlayInterval), cancellationToken: canceller);
            }
        }

        public void Play()
        {
            //最大待機量を超えたら再生しない
            if (scheduledCount >= maxQueueCount)
                return;
            
            //再生中じゃないときはそのまま再生
            if (!audioSource.isPlaying)
            {
                audioSource.PlayOneShot(assignSound);
                return;
            }

            //スケジュールする
            scheduledCount++;
        }
    }
}