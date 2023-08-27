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
        [SerializeField] private float randomRange = 1.0f;
        private Vector3[] pointsBuffer;

        /// <summary>
        /// ワーカーのスポーン座標を返します
        /// </summary>
        /// <param name="count">返すスポーンポイントの数</param>
        /// <returns>スポーン座標のコレクション</returns>
        public Span<Vector3> GetSpawnPoints()
        {
            if (pointsBuffer == null || pointsBuffer.Length == 0)
            {
                pointsBuffer = new Vector3[layers.Sum(layer => layer.count) + 1];
            }

            var center = transform.position;
            var index = 0;

            //中心
            pointsBuffer[index] = Randomize(center);
            index++;

            foreach (var layer in layers)
            {
                //1周分
                for (var i = 0; i < layer.count; i++)
                {
                    var angle = i * (360.0f / layer.count) * Mathf.Deg2Rad;
                    var x = center.x + layer.radius * Mathf.Cos(angle);
                    var z = center.z + layer.radius * Mathf.Sin(angle);

                    //ちょっとだけ座標ずらす
                    pointsBuffer[index] = Randomize(center + new Vector3(x, 0f, z));
                    index++;
                }
            }

            return pointsBuffer.AsSpan();
        }

        private Vector3 Randomize(Vector3 position)
        {
            float xOffset = Random.Range(-randomRange, randomRange);
            float zOffset = Random.Range(-randomRange, randomRange);

            return new Vector3(position.x + xOffset, 0f, position.z + zOffset);
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
                    var angle = i * (360.0f / layer.count) * Mathf.Deg2Rad;
                    var x = center.x + layer.radius * Mathf.Cos(angle);
                    var z = center.z + layer.radius * Mathf.Sin(angle);

                    Gizmos.DrawSphere(center + new Vector3(x, 0f, z), 0.3f);
                }
            }
        }
    }
}