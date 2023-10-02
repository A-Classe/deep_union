using Module.Assignment.Utility;
using UnityEngine;
using Wanna.DebugEx;

namespace Module.Assignment.Component
{
    /// <summary>
    /// 楕円判定をビジュアライズするクラス
    /// </summary>
    public class EllipseVisualizer : MonoBehaviour
    {
        [SerializeField] private float debugHeight = 1f;
        [SerializeField] private int debugResolution = 50;
        [SerializeField] private bool debugRotation;

        private EllipseData ellipseData;

        public void SetEllipse(EllipseData ellipseData)
        {
            this.ellipseData = ellipseData;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;

            var angleStep = 360f / debugResolution;

            Vector3 prevPoint = Vector3.zero;

            for (var i = 0; i <= debugResolution; i++)
            {
                var angle = i * angleStep + ellipseData.Rotation;
                var position = transform.position;

                var x = position.x + ellipseData.Size.x * 0.5f * Mathf.Cos(angle * Mathf.Deg2Rad);
                var z = position.z + ellipseData.Size.y * 0.5f * Mathf.Sin(angle * Mathf.Deg2Rad);
                
                var currentPoint = new Vector3(x, position.y + debugHeight, z);
                
                if (debugRotation)
                {
                    DebugEx.Log(currentPoint);
                }

                if (i > 0)
                {
                    Gizmos.DrawLine(prevPoint, currentPoint);
                }

                prevPoint = currentPoint;
            }
        }
    }
}