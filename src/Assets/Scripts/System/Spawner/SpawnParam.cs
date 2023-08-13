using UnityEngine;

namespace System.Spawner
{
    [CreateAssetMenu(menuName = "SpawnParam")]
    public class SpawnParam : ScriptableObject
    {
        public int SpawnCount = 50;
        public float SpawnRange = 2f;
    }
}