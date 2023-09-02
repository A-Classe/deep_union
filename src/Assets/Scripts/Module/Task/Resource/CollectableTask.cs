using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using GameMain.Presenter;
using UnityEngine;
using Wanna.DebugEx;

namespace Module.Task
{
    /// <summary>
    /// 資源を一時的に収集するクラス
    /// </summary>
    [RequireComponent(typeof(BaseTask))]
    public class CollectableTask : MonoBehaviour
    {
        [SerializeField] private GameParam gameParam;
        [SerializeField] private int resourceCount;

        private BaseTask targetTask;
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

                if (state == TaskState.Completed)
                {
                    //完了時は余ったリソースも送る(仕様としては例外的)
                    SendResource();
                }
            }
        }


        private async UniTaskVoid StartCollection(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                //収集間隔はワーカー数 * 任意の係数

                await WaitForCollectionStep(cancellationToken);

                resourceCount += 1;

                if (resourceCount >= gameParam.TemoraryStrageCount)
                {
                    SendResource();
                }
            }
        }

        private async UniTask WaitForCollectionStep(CancellationToken cancellationToken)
        {
            float checkTime = 0f;

            await UniTask.WaitUntil(() =>
            {
                checkTime += Time.deltaTime;

                return checkTime >= gameParam.CollectFactor / targetTask.WorkerCount;
            }, cancellationToken: cancellationToken);
        }

        private void SendResource()
        {
            OnCollected?.Invoke(resourceCount);
            resourceCount = 0;
        }
    }
}