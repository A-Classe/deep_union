using System.Collections.Generic;
using UnityEngine;

namespace Module.Working.Factory
{
    public class SpawnPoint : MonoBehaviour
    {
        [SerializeField] private SpawnParam spawnParam;

        [System.Serializable]
        public class LayerSettings
        {
            public GameObject objectToPlace;
            public int numberOfObjects = 10;
            public float radius = 5.0f;
            public float maxOffset = 1.0f; // 追加される最大オフセット
        }

        public List<LayerSettings> layers = new List<LayerSettings>();
        public Vector3 centerPosition;

        private void Start()
        {
            PlaceObjectsInLayers();
        }

        private void PlaceObjectsInLayers()
        {
            foreach (LayerSettings layer in layers)
            {
                for (int i = 0; i < layer.numberOfObjects; i++)
                {
                    float angle = i * (360.0f / layer.numberOfObjects);
                    float x = centerPosition.x + layer.radius * Mathf.Cos(Mathf.Deg2Rad * angle);
                    float z = centerPosition.z + layer.radius * Mathf.Sin(Mathf.Deg2Rad * angle);

                    // ランダムなオフセットを生成
                    float xOffset = Random.Range(-layer.maxOffset, layer.maxOffset);
                    float zOffset = Random.Range(-layer.maxOffset, layer.maxOffset);

                    Vector3 spawnPosition = new Vector3(x + xOffset, centerPosition.y, z + zOffset);

                    Instantiate(layer.objectToPlace, spawnPosition, Quaternion.identity);
                }
            }
        }
    }
}