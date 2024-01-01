using Core.NavMesh;
using Cysharp.Threading.Tasks;
using Module.Task;
using UnityEngine;
using VContainer;

namespace Module.Extension.Task
{
    /// <summary>
    ///     リソースを集めるクラス
    /// </summary>
    public class ResourceTask : BaseTask
    {
        private RuntimeNavMeshBaker navMeshBaker;
        [SerializeField] private GameObject[] bodyObjects;

        public override void Initialize(IObjectResolver container)
        {
            navMeshBaker = container.Resolve<RuntimeNavMeshBaker>();
        }

        protected override void OnComplete()
        {
            foreach (GameObject bodyObject in bodyObjects)
            {
                bodyObject.SetActive(false);
            }

            navMeshBaker.Bake().Forget();
        }
    }
}