using Cysharp.Threading.Tasks;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using VContainer;
using Wanna.DebugEx;

namespace Core.NavMesh
{
    public class RuntimeNavMeshBaker
    {
        private readonly NavMeshSurface navMeshSurface;

        [Inject]
        public RuntimeNavMeshBaker()
        {
            navMeshSurface = Object.FindObjectOfType<NavMeshSurface>();
        }

        public void Build()
        {
            UnityEngine.AI.NavMesh.RemoveAllNavMeshData();

            DebugEx.Assert(navMeshSurface != null, "NavMeshSurfaceがアタッチされていません! 地面のオブジェクトにNavMeshSurfaceコンポーネントが必要です。");

            navMeshSurface.BuildNavMesh();
        }

        public async UniTask Bake()
        {
            NavMeshData navMeshData = navMeshSurface.navMeshData;
            await navMeshSurface.UpdateNavMesh(navMeshData);
        }
    }
}