using UnityEngine;

namespace GameSystem
{
    public class SpawnPoint : MonoBehaviour
    {
        [SerializeField] private float spawnRange;

        public float SpawnRange => spawnRange;
    }
}