using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Wanna.DebugEx;
using Random = UnityEngine.Random;

namespace Module.Working.Factory
{
    public class SpawnPoint : MonoBehaviour
    {
        [System.Serializable]
        public class LayerSettings
        {
            public int count = 10;
            public float radius = 5.0f;
        }

        [SerializeField] private List<LayerSettings> layers;
        [SerializeField] float randomRange = 1.0f;

        public IEnumerable<Vector3> GetSpawnPoints(int count)
        {
            if (layers.Sum(layer => layer.count) < count)
            {
                DebugEx.LogError("設定されたスポーンポイントをオーバーしています");
                yield break;
            }

            Vector3 center = transform.position;
            int currentCount = 1;

            yield return Randomize(center);

            foreach (LayerSettings layer in layers)
            {
                for (int i = 0; i < layer.count; i++)
                {
                    float angle = i * (360.0f / layer.count) * Mathf.Deg2Rad;
                    float x = center.x + layer.radius * Mathf.Cos(angle);
                    float z = center.z + layer.radius * Mathf.Sin(angle);

                    yield return Randomize(center + new Vector3(x, 0f, z));

                    if (currentCount == count)
                        yield break;
                }
            }

            Vector3 Randomize(Vector3 position)
            {
                float xOffset = Random.Range(-randomRange, randomRange);
                float zOffset = Random.Range(-randomRange, randomRange);

                return new Vector3(position.x + xOffset, 0f, position.z + zOffset);
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;

            Vector3 center = transform.position;

            Gizmos.DrawSphere(center, 0.3f);

            foreach (LayerSettings layer in layers)
            {
                for (int i = 0; i < layer.count; i++)
                {
                    float angle = i * (360.0f / layer.count) * Mathf.Deg2Rad;
                    float x = center.x + layer.radius * Mathf.Cos(angle);
                    float z = center.z + layer.radius * Mathf.Sin(angle);

                    Gizmos.DrawSphere(center + new Vector3(x, 0f, z), 0.3f);
                }
            }
        }
    }
}