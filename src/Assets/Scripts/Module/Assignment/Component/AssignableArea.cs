using System;
using System.Collections.Generic;
using System.Linq;
using Core.Utility;
using Module.Assignment.Utility;
using Module.Working;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;
using Wanna.DebugEx;

namespace Module.Assignment.Component
{
    /// <summary>
    /// アサイン範囲を設定するクラス
    /// </summary>
    public class AssignableArea : MonoBehaviour
    {
        [SerializeField] private DecalProjector lightProjector;

        [FormerlySerializedAs("ellipseCollider")]
        [SerializeField]
        private EllipseVisualizer ellipseVisualizer;

        [SerializeField] private float intensity;

        [Header("回転させる場合はこっちをいじる！")]
        [SerializeField]
        private float rotation;

        [SerializeField] private float2 size;
        [SerializeField] private float2 factor;
        [SerializeField] private bool debugAssignPoints;

        [SerializeField] private GameObject assignPointPrefab;
        [SerializeField] private Transform pointParent;

        public float Intensity => intensity;

        public EllipseData EllipseData => new EllipseData(transform.position, size * factor, rotation);
        private EllipseData ellipseData;

        public IReadOnlyList<Worker> AssignedWorkers => assignedWorkers;
        private List<Worker> assignedWorkers;

        private AutoInstanceList<AssignPoint> assignPoints;
        private Light areaLight;
        private Material lightMaterial;

        private static readonly int IntensityKey = Shader.PropertyToID("_Intensity");

        public event Action<Worker> OnWorkerEnter;
        public event Action<Worker> OnWorkerExit;

        private void Awake()
        {
            assignPoints = new AutoInstanceList<AssignPoint>(assignPointPrefab, pointParent, 20, size / 2);
            assignPoints.SetList(GetComponentsInChildren<AssignPoint>().ToList());
            assignedWorkers = new List<Worker>();

            //Decalだと自動で複製されないので新しいインスタンスを作る
            Material newMaterial = new Material(lightProjector.material);
            lightProjector.material = newMaterial;
            lightMaterial = newMaterial;

            SetLightIntensity(intensity);
        }

        private void OnValidate()
        {
            SetEnableAssignPointDebug(debugAssignPoints);
            SetLightSize();
            ellipseVisualizer.SetEllipse(ellipseData);
        }

        private void SetLightSize()
        {
            ellipseData = new EllipseData(transform.position, size * factor, rotation);

            Vector3 eulerAngles = transform.localRotation.eulerAngles;
            eulerAngles.y = rotation;
            transform.localRotation = Quaternion.Euler(eulerAngles);

            lightProjector.size = new Vector3(size.x, size.y, 10f);
        }

        public void SetLightIntensity(float intensity)
        {
            this.intensity = intensity;
            lightMaterial.SetFloat(IntensityKey, intensity);
        }

        private void ReleaseAssignPoint(Transform assignPoint)
        {
            assignPoints.Add(assignPoint.GetComponent<AssignPoint>());
        }

        public bool CanAssign()
        {
            if (assignPoints.Count == 0)
            {
                DebugEx.LogWarning("登録できるAssignPointはありません！");
            }

            return assignPoints.Count > 0;
        }

        private Transform GetNearestAssignPoint(Vector3 target)
        {
            if (assignPoints.Count == 0)
                return null;

            assignPoints.Sort((a, b) =>
                {
                    Vector3 p1 = target - a.transform.position;
                    Vector3 p2 = target - b.transform.position;
                    float distance = p1.sqrMagnitude - p2.sqrMagnitude;

                    if (distance > 0)
                    {
                        return 1;
                    }

                    if (distance < 0)
                    {
                        return -1;
                    }

                    return 0;
                }
            );


            Transform assignPoint = assignPoints[0].transform;
            assignPoints.RemoveAt(0);

            return assignPoint;
        }

        public void AddWorker(Worker worker)
        {
            Transform assignPoint = GetNearestAssignPoint(worker.transform.position);

            worker.SetFollowTarget(transform, assignPoint.transform);
            assignedWorkers.Add(worker);
            OnWorkerEnter?.Invoke(worker);
        }

        public void RemoveWorker(Worker worker)
        {
            assignedWorkers.Remove(worker);
            OnWorkerExit?.Invoke(worker);
            ReleaseAssignPoint(worker.Target);
        }

        void SetEnableAssignPointDebug(bool enable)
        {
            assignPoints = new AutoInstanceList<AssignPoint>(assignPointPrefab, pointParent, 20, size / 2);
            assignPoints.SetList(GetComponentsInChildren<AssignPoint>().ToList());

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