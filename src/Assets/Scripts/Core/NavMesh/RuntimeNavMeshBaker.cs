using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;
using VContainer;

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
            navMeshSurface.BuildNavMesh();
        }

        public async UniTask Bake()
        {
            NavMeshData navMeshData = navMeshSurface.navMeshData;
            await navMeshSurface.UpdateNavMesh(navMeshData);
        }
    }
}