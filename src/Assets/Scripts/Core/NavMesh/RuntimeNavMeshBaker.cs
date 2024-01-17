using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;
using VContainer;
using Wanna.DebugEx;

namespace Core.NavMesh
{
    /// <summary>
    /// ランタイムでNavMeshを生成するクラス
    /// </summary>
    public class RuntimeNavMeshBaker
    {
        private readonly NavMeshSurface navMeshSurface;

        [Inject]
        public RuntimeNavMeshBaker()
        {
            //シーン上のNavMeshSurfaceを取得
            navMeshSurface = Object.FindObjectOfType<NavMeshSurface>();
        }

        public void Build()
        {
            //現在登録されているNavMshDataを削除
            UnityEngine.AI.NavMesh.RemoveAllNavMeshData();

            DebugEx.Assert(navMeshSurface != null, "NavMeshSurfaceがアタッチされていません! 地面のオブジェクトにNavMeshSurfaceコンポーネントが必要です。");

            //NavMeshを構築
            navMeshSurface.BuildNavMesh();
        }

        public async UniTask Bake()
        {
            NavMeshData navMeshData = navMeshSurface.navMeshData;
            await navMeshSurface.UpdateNavMesh(navMeshData);
        }
    }
}