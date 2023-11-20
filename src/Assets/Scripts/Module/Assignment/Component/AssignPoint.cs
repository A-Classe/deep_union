using UnityEngine;

namespace Module.Assignment.Component
{
    /// <summary>
    ///     アサインするポイントを設定するコンポーネント
    /// </summary>
    public class AssignPoint : MonoBehaviour
    {
        private void OnDrawGizmos()
        {
            if (!enabled)
                return;

            Gizmos.color = Color.red;
            Gizmos.DrawSphere(transform.position, 0.25f);
        }
    }
}