using System;
using System.Collections.Generic;
using Module.Task;
using UI.InGame;
using UnityEngine;
using UnityEngine.AI;
using VContainer;

namespace Module.Minimap
{
    public class MiniMapBuilder
    {
        
        private readonly InGameUIManager inGameUIManager;
        

        private readonly TaskActivator taskActivator;
        
        private struct MeshData
        {
            public Vector3[] Vertices;
            public int[] Triangles;
        }
        
        private const float CutOffHeight = 4f;
        
        
        
        [Inject]
        public MiniMapBuilder(
            InGameUIManager inGameUIManager,
            TaskActivator taskActivator
        )
        {
            this.inGameUIManager = inGameUIManager;

            this.taskActivator = taskActivator;
        }
        
        private void SetActiveAreas()
        {
            ReadOnlySpan<BaseTask> tasks = taskActivator.GetActiveTasks();
            //タスクのアサインエリアを登録
            foreach (BaseTask task in tasks)
            {
                Debug.Log(task);
            }
            Debug.Log(tasks.Length);
        }

        public void Build()
        {
             BaseTask[] tasks = TaskUtil.FindSceneTasks<BaseTask>();
             Debug.Log(tasks.Length);
            BuildMeshAsync();
        }

        private async void BuildMeshAsync()
        {
            NavMeshTriangulation triangulation = NavMesh.CalculateTriangulation();
            MeshData meshData = await System.Threading.Tasks.Task.Run(() => GenerateMesh(triangulation));
            SetupMesh(meshData);
        }

        MeshData GenerateMesh(NavMeshTriangulation triangulation)
        {
            Vector3[] originalVertices = triangulation.vertices;
            int[] originalIndices = triangulation.indices;

            List<Vector3> filteredVertices = new List<Vector3>();
            List<int> filteredIndices = new List<int>();

            // 頂点の辞書を作成（元の頂点インデックス -> 新しい頂点インデックス）
            Dictionary<int, int> vertexIndexMapping = new Dictionary<int, int>();

            // 頂点をフィルタリングし、マッピングを作成
            for (int i = 0; i < originalVertices.Length; i++)
            {
                if (originalVertices[i].y <= CutOffHeight)
                {
                    vertexIndexMapping.Add(i, filteredVertices.Count);
                    filteredVertices.Add(originalVertices[i]);
                }
            }

            // 三角形のインデックスをフィルタリングし、再マッピング
            for (int i = 0; i < originalIndices.Length; i += 3)
            {
                if (vertexIndexMapping.ContainsKey(originalIndices[i]) &&
                    vertexIndexMapping.ContainsKey(originalIndices[i + 1]) &&
                    vertexIndexMapping.ContainsKey(originalIndices[i + 2]))
                {
                    filteredIndices.Add(vertexIndexMapping[originalIndices[i]]);
                    filteredIndices.Add(vertexIndexMapping[originalIndices[i + 1]]);
                    filteredIndices.Add(vertexIndexMapping[originalIndices[i + 2]]);
                }
            }
            
            return new MeshData
            {
                Vertices = filteredVertices.ToArray(),
                Triangles = filteredIndices.ToArray()
            };
        }

        private void SetupMesh(MeshData meshData)
        {
            Mesh mesh = new Mesh();
            mesh.vertices = meshData.Vertices;
            mesh.triangles = meshData.Triangles;
            mesh.RecalculateBounds();
            // メッシュを表示するオブジェクトを作成
            GameObject meshObj = new GameObject("NavMeshRepresentation");
            meshObj.layer = LayerMask.NameToLayer("UI");
            meshObj.transform.position = Vector3.zero; // 位置は必要に応じて調整

            // MeshFilterとMeshRendererを追加
            MeshFilter meshFilter = meshObj.AddComponent<MeshFilter>();
            meshFilter.mesh = mesh;
            MeshRenderer meshRenderer = meshObj.AddComponent<MeshRenderer>();

            // メッシュにマテリアルを設定 (デフォルトマテリアルを使用)
            meshRenderer.material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        }
        
    }
}