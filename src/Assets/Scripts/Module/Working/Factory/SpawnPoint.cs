using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Wanna.DebugEx;
using Random = UnityEngine.Random;

namespace Module.Working.Factory
{
    /// <summary>
    /// ワーカーのスポーン座標を生成するクラス
    /// </summary>
    public class SpawnPoint : MonoBehaviour
    {
        [Serializable]
        public class LayerSettings
        {
            public int count = 10;
            public float radius = 5.0f;
        }

        [SerializeField] private List<LayerSettings> layers;
        [SerializeField] float randomRange = 1.0f;
        private Vector3[] pointsBuffer;

        private void Start()
        {
            pointsBuffer = new Vector3[layers.Sum(layer => layer.count)];
        }

        /// <summary>
        /// ワーカーのスポーン座標を返します
        /// </summary>
        /// <param name="count">返すスポーンポイントの数</param>
        /// <returns>スポーン座標のコレクション</returns>
        public Span<Vector3> GetSpawnPoints(int count)
        {
            if (layers.Sum(layer => layer.count) < count)
            {
                DebugEx.LogError("設定されたスポーンポイントをオーバーしています");
                return Span<Vector3>.Empty;
            }

            Vector3 center = transform.position;
            int index = 1;

            //中心
            pointsBuffer[index] = Randomize(center);

            foreach (LayerSettings layer in layers)
            {
                //1周分
                for (int i = 0; i < layer.count; i++)
                {
                    float angle = i * (360.0f / layer.count) * Mathf.Deg2Rad;
                    float x = center.x + layer.radius * Mathf.Cos(angle);
                    float z = center.z + layer.radius * Mathf.Sin(angle);

                    //ちょっとだけ座標ずらす
                    pointsBuffer[index] = Randomize(center + new Vector3(x, 0f, z));
                    index++;

                    if (index == count)
                    {
                        return pointsBuffer.AsSpan().Slice(0, count);
                    }
                }
            }

            Vector3 Randomize(Vector3 position)
            {
                float xOffset = Random.Range(-randomRange, randomRange);
                float zOffset = Random.Range(-randomRange, randomRange);

                return new Vector3(position.x + xOffset, 0f, position.z + zOffset);
            }

            return Span<Vector3>.Empty;
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