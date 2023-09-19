using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class RemoveMissingScripts : UnityEditor.Editor
    {
        [MenuItem("Tools/Remove Missing Scripts")]
        public static void Remove()
        {
            var objs = Resources.FindObjectsOfTypeAll<GameObject>();
            int count = objs.Sum(GameObjectUtility.RemoveMonoBehavioursWithMissingScript);
            Debug.Log($"Removed {count} missing scripts");
        }
    }
}