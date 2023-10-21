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