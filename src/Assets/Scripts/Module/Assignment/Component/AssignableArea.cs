using System;
using System.Collections.Generic;
using System.Linq;
using Core.Utility;
using Module.Assignment.Utility;
using Module.Working;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Module.Assignment.Component
{
    /// <summary>
    ///     アサイン範囲を設定するクラス
    /// </summary>
    public class AssignableArea : MonoBehaviour
    {
        [SerializeField] private bool isAssignable = true;
        [SerializeField] AssignableAreaLight areaLight;

        [SerializeField] private float2 size;
        [SerializeField] private float2 factor;
        [SerializeField] private bool debugAssignPoints;
        [SerializeField] private GameObject assignPointPrefab;
        [SerializeField] private Transform pointParent;
        
        public EllipseData AreaShape => new(transform.position, size * factor, transform.eulerAngles.y);
        
        private AutoInstanceList<AssignPoint> assignPoints;
        private List<Worker> assignedWorkers;
        public IReadOnlyList<Worker> AssignedWorkers => assignedWorkers;

        private void Awake()
        {
            assignPoints = new AutoInstanceList<AssignPoint>(assignPointPrefab, pointParent, 20, size / 2);
            assignPoints.SetList(GetComponentsInChildren<AssignPoint>().ToList());
            assignedWorkers = new List<Worker>();
        }

        private void OnValidate()
        {
            SetEnableAssignPointDebug(debugAssignPoints);
            areaLight.SetLightSize(size);
        }

        public event Action<Worker, WorkerEventType> OnWorkerEnter;
        public event Action<Worker, WorkerEventType> OnWorkerExit;

        private void ReleaseAssignPoint(Transform assignPoint)
        {
            assignPoints.Add(assignPoint.GetComponent<AssignPoint>());
        }

        public bool CanAssign()
        {
            if (assignPoints.Count == 0)
            {
                //DebugEx.LogWarning("登録できるAssignPointはありません！");
            }

            return isAssignable && assignPoints.Count > 0;
        }

        /// <summary>
        /// 指定された座標から最も近いアサインポイントを返します。
        /// </summary>
        /// <param name="target">指定座標</param>
        /// <returns>アサインポイントのTransform</returns>
        private Transform GetNearestAssignPoint(Vector3 target)
        {
            if (assignPoints.Count == 0)
            {
                return null;
            }

            var minDistance = float.MaxValue;
            AssignPoint nearestPoint = null;

            //最短距離を探索
            foreach (var point in assignPoints)
            {
                var distance = (point.transform.position - target).sqrMagnitude;

                if (minDistance > distance)
                {
                    minDistance = distance;
                    nearestPoint = point;
                }
            }

            assignPoints.Remove(nearestPoint);

            return nearestPoint.transform;
        }

        public enum WorkerEventType
        {
            Create,
            Destroy,
            Default
        }

        /// <summary>
        /// ワーカーをエリアに追加します
        /// </summary>
        /// <param name="worker">ワーカー</param>
        /// <param name="type">ワーカーイベント</param>
        public void AddWorker(Worker worker, WorkerEventType type = WorkerEventType.Default)
        {
            var assignPoint = GetNearestAssignPoint(worker.transform.position);

            worker.SetFollowTarget(transform, assignPoint.transform);
            assignedWorkers.Add(worker);
            OnWorkerEnter?.Invoke(worker, type);

            worker.OnDead += OnWorkerDead;
        }

        /// <summary>
        /// ワーカーをエリアから削除します
        /// </summary>
        /// <param name="worker">ワーカー</param>
        /// <param name="type">ワーカーイベント</param>
        public void RemoveWorker(Worker worker, WorkerEventType type = WorkerEventType.Default)
        {
            assignedWorkers.Remove(worker);
            OnWorkerExit?.Invoke(worker, type);
            ReleaseAssignPoint(worker.Target);

            worker.OnDead -= OnWorkerDead;
        }

        private void OnWorkerDead(Worker worker)
        {
            RemoveWorker(worker, WorkerEventType.Destroy);
        }

        private void SetEnableAssignPointDebug(bool enable)
        {
            assignPoints = new AutoInstanceList<AssignPoint>(assignPointPrefab, pointParent, 20, size / 2);
            assignPoints.SetList(GetComponentsInChildren<AssignPoint>().ToList());

            //アサインポイントのデブッグを有効化する
            foreach (var assignPoint in assignPoints)
            {
                if (assignPoint == null)
                {
                    return;
                }

                assignPoint.enabled = enable;
            }
        }
    }
}