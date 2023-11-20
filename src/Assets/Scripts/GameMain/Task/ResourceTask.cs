using Core.NavMesh;
using Cysharp.Threading.Tasks;
using Module.Task;
using VContainer;

namespace GameMain.Task
{
    /// <summary>
    ///     リソースを集めるクラス
    /// </summary>
    public class ResourceTask : BaseTask
    {
        private RuntimeNavMeshBaker navMeshBaker;

        public override void Initialize(IObjectResolver container)
        {
            navMeshBaker = container.Resolve<RuntimeNavMeshBaker>();
        }

        protected override void OnComplete()
        {
            Disable();
            navMeshBaker.Bake().Forget();
        }
    }
}