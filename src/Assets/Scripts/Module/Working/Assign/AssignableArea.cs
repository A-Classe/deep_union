using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using Wanna.DebugEx;

namespace Module.Task
{
    public class AssignableArea : MonoBehaviour
    {
        [SerializeField] private EllipseCollider ellipseCollider;
        [SerializeField] private Vector2 areaRadius;
        [SerializeField] private bool debugAssignPoints;

        [Header("必要な時のみアタッチ")]
        [SerializeField]
        private AssignableAreaLight assignableAreaLight;

        private List<AssignPoint> assignPoints;
        private Light areaLight;

        private void Awake()
        {
            assignPoints = GetComponentsInChildren<AssignPoint>().ToList();
        }

        private void OnValidate()
        {
            SetEnableAssignPointDebug(debugAssignPoints);
            ellipseCollider.SetSize(areaRadius * 2f);

            if (assignableAreaLight != null)
            {
                assignableAreaLight.AdjustAreaLight(areaRadius.x);
            }
        }

        public void ReleaseAssignPoint(Transform assignPoint)
        {
            assignPoints.Add(assignPoint.GetComponent<AssignPoint>());
        }

        public bool TryGetNearestAssignPoint(Vector3 target, out Transform assignPoint)
        {
            //アサインできる座標がなかったらアサイン不可
            if (assignPoints.Count == 0)
            {
                assignPoint = null;
                return false;
            }

            assignPoints.Sort((a, b) =>
                {
                    Vector3 p1 = target - a.transform.position;
                    Vector3 p2 = target - b.transform.position;

                    if (p1.sqrMagnitude - p2.sqrMagnitude > 0)
                    {
                        return 1;
                    }

                    return -1;
                }
            );

            assignPoint = assignPoints[0].transform;
            assignPoints.RemoveAt(0);

            return true;
        }

        void SetEnableAssignPointDebug(bool enable)
        {
            assignPoints = GetComponentsInChildren<AssignPoint>().ToList();

            //アサインポイントのデブッグを有効化する
            foreach (AssignPoint assignPoint in assignPoints)
            {
                if (assignPoint == null)
                    return;

                assignPoint.enabled = enable;
            }
        }
    }
}