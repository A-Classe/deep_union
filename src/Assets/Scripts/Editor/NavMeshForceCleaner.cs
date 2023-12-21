using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

namespace Editor
{
    public class NavMeshForceCleaner : MonoBehaviour
    {
        [MenuItem("Tools/Force Cleanup NavMesh")]
        public static void ForceCleanupNavMesh()
        {
            if (Application.isPlaying)
            {
                return;
            }

            NavMesh.RemoveAllNavMeshData();
        }
    }
}