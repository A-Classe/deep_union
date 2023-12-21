using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using GameMain.Presenter;
using Module.Assignment.Component;
using Module.Player.Controller;
using Module.Task;
using UnityEngine;
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
        [SerializeField] private Vector3 targetOffset;
        [SerializeField] private int resourceCount;
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
            itemComponent.SetCollideEvent(() => OnCollected?.Invoke(countCache));

            var rig = item.GetComponent<Rigidbody>();
            var target = playerController.transform.position;
            target += targetOffset;
            var velocity = CalculateForce(item.transform.position, target, 45f);
            rig.AddForce(velocity * rig.mass, ForceMode.Impulse);
        }

        private Vector3 CalculateForce(Vector3 start, Vector3 target, float angle)
        {
            var rad = angle * Mathf.Deg2Rad;

            var x = Vector2.Distance(new Vector2(start.x, start.z), new Vector2(target.x, target.z));
            var y = start.y - target.y;

            var speed = Mathf.Sqrt(-Physics.gravity.y * Mathf.Pow(x, 2) /
                                   (2 * Mathf.Pow(Mathf.Cos(rad), 2) * (x * Mathf.Tan(rad) + y)));

            return new Vector3(target.x - start.x, x * Mathf.Tan(rad), target.z - start.z).normalized * speed;
        }
    }
}