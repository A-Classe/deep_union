using Module.Assignment.Utility;
using UnityEngine;

namespace Module.Assignment.Component
{
    /// <summary>
    /// 楕円判定をビジュアライズするクラス
    /// </summary>
    public class EllipseVisualizer : MonoBehaviour
    {
        [SerializeField] private float debugHeight = 1f;
        [SerializeField] private int debugResolution = 50;

        private EllipseData ellipseData;

        public void SetEllipse(EllipseData ellipseData)
        {
            this.ellipseData = ellipseData;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;

            var angleStep = 2f * Mathf.PI / debugResolution;
            var center = transform.position;
            var xRadius = ellipseData.Size.x * 0.5f;
            var yRadius = ellipseData.Size.y * 0.5f;

            Quaternion rotation = Quaternion.AngleAxis(ellipseData.Rotation, Vector3.up);
            Vector3 startPoint = center + new Vector3(Mathf.Cos(0f) * xRadius, debugHeight, Mathf.Sin(0f) * yRadius);

            for (int i = 1; i <= debugResolution; i++)
            {
                float angle = i * angleStep;
                Vector3 endPoint = center + new Vector3(Mathf.Cos(angle) * xRadius, debugHeight, Mathf.Sin(angle) * yRadius);
                Gizmos.DrawLine(Rotate(startPoint), Rotate(endPoint));
                startPoint = endPoint;
            }

            Vector3 Rotate(Vector3 point)
            {
                return rotation * (point - center) + center;
            }
        }
    }
}