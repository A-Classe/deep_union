using System;
using UnityEngine;

namespace Module.Task
{
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