using System;
using System.Collections.Generic;
using System.Linq;
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
        [SerializeField] private EllipseCollider ellipseCollider;
        [SerializeField] private float2 size;
        [SerializeField] private float2 factor;
        [SerializeField] private float intensity;
        [SerializeField] private bool debugAssignPoints;

        private LightData lightData;

        private List<AssignPoint> assignPoints;
        private Light areaLight;
        private Material lightMaterial;

        private static readonly int IntensityKey = Shader.PropertyToID("_Intensity");

        private void Awake()
        {
            assignPoints = GetComponentsInChildren<AssignPoint>().ToList();

            //Decalだと自動で複製されないので新しいインスタンスを作る
            Material newMaterial = new Material(lightMaterial);
            lightProjector.material = newMaterial;
            lightMaterial = newMaterial;
        }

        private void OnValidate()
        {
            lightMaterial = lightProjector.material;

            SetEnableAssignPointDebug(debugAssignPoints);
            SetLightSize();
            ellipseCollider.SetSize(lightData.Size);

            SetLightIntensity();
        }

        public void SetLightSize()
        {
            lightData = new LightData(size * factor, intensity);
            lightProjector.size = new Vector3(size.x, size.y, 10f);
        }

        public void SetLightIntensity()
        {
            lightData = new LightData(size * factor, intensity);
            lightMaterial.SetFloat(IntensityKey, lightData.Intensity);
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