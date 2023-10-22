using UnityEngine;

namespace System.GameProgress
{
    /// <summary>
    /// ゴール地点を設定するComponent
    /// </summary>
    [ExecuteInEditMode]
    public class GoalPoint : MonoBehaviour
    {
        private RectTransform rect;

        private void OnValidate()
        {
            rect = GetComponent<RectTransform>();
        }
        
        private void Start()
        {
            rect = GetComponent<RectTransform>();
        }

        private void Update()
        {
            //yz軸に強制
            rect.position = Vector3.Scale(rect.position, new Vector3(0f, 1f, 1f));
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;

            Vector3 position = transform.position + Vector3.down;
            Gizmos.DrawLine(position, position + Vector3.down * 10f);
        }
    }
}