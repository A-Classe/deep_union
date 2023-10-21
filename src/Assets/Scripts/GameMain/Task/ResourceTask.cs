using Core.NavMesh;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Module.Assignment.Component;
using Module.Player.Controller;
using Module.Task;
using Module.Working;
using UnityEngine;
using VContainer;

namespace GameMain.Task
{
    /// <summary>
    /// リソースを集めるクラス
    /// </summary>
    public class ResourceTask : BaseTask
    {
        [SerializeField] private AssignableArea assignableArea;
        [SerializeField] private CollectableTask collectableTask;
        [SerializeField] private GameObject resourceItem;
        private RuntimeNavMeshBaker navMeshBaker;
        private PlayerController playerController;

        public override void Initialize(IObjectResolver container)
        {
            navMeshBaker = container.Resolve<RuntimeNavMeshBaker>();
            playerController = container.Resolve<PlayerController>();

            collectableTask.OnCollected += count =>
            {
                Worker worker = assignableArea.AssignedWorkers[Random.Range(0, assignableArea.AssignedWorkers.Count)];
                GameObject item = Instantiate(resourceItem, worker.transform.position, Quaternion.identity);
            };
        }

        protected override void OnComplete()
        {
            Disable();
            navMeshBaker.Bake().Forget();
        }
    }
}