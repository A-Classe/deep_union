using System;
using System.Collections.Generic;
using System.Linq;
using Module.Working;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;
using Wanna.DebugEx;

namespace Module.Task
{
    public class AssignableArea : MonoBehaviour
    {
        [SerializeField] private DecalProjector lightProjector;

        [FormerlySerializedAs("ellipseCollider")]
        [SerializeField]
        private EllipseVisualizer ellipseVisualizer;

        [SerializeField] private float intensity;
        [SerializeField] private short priority;
        [SerializeField] private float2 size;
        [SerializeField] private float2 factor;
        [SerializeField] private bool debugAssignPoints;

        public LightData LightData => lightData;
        private LightData lightData;

        private List<AssignPoint> assignPoints;
        private Light areaLight;
        private Material lightMaterial;

        private static readonly int IntensityKey = Shader.PropertyToID("_Intensity");

        public event Action<Worker> OnWorkerEnter;
        public event Action<Worker> OnWorkerExit;

        private void Awake()
        {
            assignPoints = GetComponentsInChildren<AssignPoint>().ToList();

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
            ellipseVisualizer.SetSize(lightData.Size);
        }

        public void SetLightSize()
        {
            lightData = new LightData(transform.position, size * factor, intensity, priority);
            lightProjector.size = new Vector3(size.x, size.y, 10f);
        }

        public void SetLightIntensity(float intensity)
        {
            this.intensity = intensity;
            lightData = new LightData(transform.position, size * factor, intensity, priority);
            lightMaterial.SetFloat(IntensityKey, lightData.Intensity);
        }

        internal void OnWorkerEnter_Internal(Worker worker)
        {
            Transform assignPoint = GetNearestAssignPoint(worker.transform.position);

            if (assignPoint == null)
                return;

            worker.SetFollowTarget(assignPoint.transform);
            OnWorkerEnter?.Invoke(worker);
        }

        internal void OnWorkerExit_Internal(Worker worker)
        {
            OnWorkerExit?.Invoke(worker);
            ReleaseAssignPoint(worker.Target);
        }

        public void ReleaseAssignPoint(Transform assignPoint)
        {
            assignPoints.Add(assignPoint.GetComponent<AssignPoint>());
        }

        public Transform GetNearestAssignPoint(Vector3 target)
        {
            if (assignPoints.Count == 0)
            {
                DebugEx.LogWarning("登録できるAssignPointはありません！");
                return null;
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


            Transform assignPoint = assignPoints[0].transform;
            assignPoints.RemoveAt(0);

            return assignPoint;
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