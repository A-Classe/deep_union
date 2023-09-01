using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using GameMain.Presenter;
using UnityEngine;

namespace Module.Task
{
    /// <summary>
    /// 資源を一時的に収集するクラス
    /// </summary>
    [RequireComponent(typeof(BaseTask))]
    public class CollectableTask : MonoBehaviour
    {
        [SerializeField] private GameParam gameParam;

        private BaseTask targetTask;
        private int resourceCount;
        private CancellationTokenSource collectCanceller;

        public event Action<int> OnCollected;

        private void Awake()
        {
            targetTask = GetComponent<BaseTask>();
            targetTask.OnStateChanged += OnStateChanged;
        }

        private void OnStateChanged(TaskState state)
        {
            if (state == TaskState.InProgress)
            {
                collectCanceller = new CancellationTokenSource();
                StartCollection(collectCanceller.Token).Forget();
            }
            else
            {
                collectCanceller?.Cancel();
                collectCanceller?.Dispose();

                //余ったリソースも送る(仕様としては例外的)
                SendResource();
            }
        }

        private async UniTaskVoid StartCollection(CancellationToken cancellationToken)
        {
            //収集間隔はワーカー数 * 任意の係数
            TimeSpan collectSpan = TimeSpan.FromSeconds(targetTask.WorkerCount * gameParam.CollectFactor);

            while (!cancellationToken.IsCancellationRequested)
            {
                await UniTask.Delay(collectSpan, cancellationToken: cancellationToken);

                resourceCount += 1;

                if (resourceCount >= gameParam.TemoraryStrageCount)
                {
                    SendResource();
                }
            }
        }

        private void SendResource()
        {
            OnCollected?.Invoke(resourceCount);
            resourceCount = 0;
        }
    }
}