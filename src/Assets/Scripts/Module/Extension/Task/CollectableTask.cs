using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using GameMain.Presenter;
using Module.Assignment.Component;
using Module.Player.Controller;
using Module.Task;
using UnityEngine;
using Wanna.DebugEx;
using Random = UnityEngine.Random;

namespace Module.Extension.Task
{
    /// <summary>
    ///     資源を一時的に収集するクラス
    /// </summary>
    [RequireComponent(typeof(BaseTask))]
    public class CollectableTask : MonoBehaviour
    {
        [SerializeField] private GameParam gameParam;
        [SerializeField] private AssignableArea assignableArea;
        [SerializeField] private GameObject resourceItem;
        [SerializeField] private int resourceCount;
        [SerializeField] private Vector3 offset;
        [SerializeField] private float throwAngle;
        [SerializeField] private float initialForce;
        private CancellationTokenSource collectCanceller;
        private PlayerController playerController;

        private BaseTask targetTask;

        private void Awake()
        {
            targetTask = GetComponent<BaseTask>();
            targetTask.OnStateChanged += OnStateChanged;

            playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        }

        public event Action<int> OnCollected;

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
                    //完了時は余ったリソースも送る(仕様としては例外的)
                {
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
                    resourceCount = 0;
                }
            }
        }

        private async UniTask WaitForCollectionStep(CancellationToken cancellationToken)
        {
            var checkTime = 0f;

            await UniTask.WaitUntil(() =>
            {
                checkTime += Time.deltaTime;

                return checkTime >= gameParam.CollectFactor / targetTask.WorkerCount;
            }, cancellationToken: cancellationToken);
        }

        private void SendResource()
        {
            var worker = assignableArea.AssignedWorkers[Random.Range(0, assignableArea.AssignedWorkers.Count)];
            var item = Instantiate(resourceItem, worker.transform.position, Quaternion.identity);

            var countCache = resourceCount;
            var itemComponent = item.GetComponent<ResourceItem>();

            //初速度を潜水艦の方向で作成
            Vector3 forward = (playerController.transform.position - item.transform.position).normalized;
            Vector3 right = Quaternion.Euler(0f, 90f, 0f) * forward;
            Vector3 initialVelocity = Quaternion.AngleAxis(-throwAngle, right) * forward * initialForce;
            itemComponent.Throw(playerController.transform, offset, initialVelocity, () => OnCollected?.Invoke(countCache)).Forget();
        }
    }
}