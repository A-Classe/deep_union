using System.Collections.Generic;
using UnityEngine;
using Wanna.DebugEx;

namespace Module.Task
{
    /// <summary>
    ///     タスクを検出するクラス
    /// </summary>
    public class TaskDetector : MonoBehaviour
    {
        [SerializeField] private float detectRadius;
        [SerializeField] private LayerMask detectLayer;

        private readonly Collider[] colliderBuffer = new Collider[32];

        private readonly List<BaseTask> detectedTasks = new();

        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(1f, 0.92f, 0.02f, 0.51f);
            var position = transform.position;
            Gizmos.DrawSphere(position, detectRadius);

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(position, detectRadius);
        }

        /// <summary>
        ///     親オブジェクトの座標に最も近いタスクを取得します
        /// </summary>
        /// <returns>検出したタスク</returns>
        public BaseTask GetNearestTask()
        {
            if (detectedTasks.Count == 0)
                return null;

            var origin = transform.position;
            var minDistance = float.MaxValue;
            BaseTask nearestTask = null;

            foreach (var task in detectedTasks)
            {
                var distance = (task.transform.position - origin).sqrMagnitude;
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestTask = task;
                }
            }

            return nearestTask;
        }

        /// <summary>
        ///     検出処理を更新します
        /// </summary>
        public void UpdateDetection()
        {
            var count = Physics.OverlapSphereNonAlloc(transform.position, detectRadius, colliderBuffer, detectLayer);

            detectedTasks.Clear();

            for (var i = 0; i < count; i++)
                if (colliderBuffer[i].transform.parent.TryGetComponent(out BaseTask baseTask))
                    detectedTasks.Add(baseTask);
        }
    }
}