using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

namespace Module.Task
{
    /// <summary>
    /// タスクを検出するクラス
    /// </summary>
    public class TaskDetector : MonoBehaviour
    {
        [SerializeField] private float detectRadius;
        [SerializeField] private LayerMask detectLayer;

        private readonly List<BaseTask> detectedTasks = new List<BaseTask>();

        /// <summary>
        /// 親オブジェクトの座標に最も近いタスクを取得します
        /// </summary>
        /// <returns>検出したタスク</returns>
        public BaseTask GetNearestTask()
        {
            if (detectedTasks.Count == 0)
                return null;

            Vector3 origin = transform.position;
            float minDistance = float.MaxValue;
            BaseTask nearestTask = null;

            foreach (BaseTask task in detectedTasks)
            {
                float distance = (task.transform.position - origin).sqrMagnitude;
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestTask = task;
                }
            }

            return nearestTask;
        }

        private readonly Collider[] colliderBuffer = new Collider[32];

        /// <summary>
        /// 検出処理を更新します
        /// </summary>
        public void UpdateDetection()
        {
            int count = Physics.OverlapSphereNonAlloc(transform.position, detectRadius, colliderBuffer, detectLayer);

            detectedTasks.Clear();

            for (int i = 0; i < count; i++)
            {
                if (colliderBuffer[i].TryGetComponent(out BaseTask baseTask))
                {
                    detectedTasks.Add(baseTask);
                }
            }
        }

        void OnDrawGizmos()
        {
            Gizmos.color = new Color(1f, 0.92f, 0.02f, 0.51f);
            Gizmos.DrawSphere(transform.position, detectRadius);

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, detectRadius);
        }
    }
}