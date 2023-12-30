using UnityEngine;

// ReSharper disable NotAccessedField.Global

namespace Module.Working.Factory
{
    [CreateAssetMenu(menuName = "SpawnParam")]
    public class SpawnParam : ScriptableObject
    {
        public int SpawnCount = 50;
        public int SpawnCapacity = 128;
        public float SpawnRange = 2f;
    }
}