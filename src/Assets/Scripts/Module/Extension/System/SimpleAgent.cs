using UnityEngine;
using Wanna.DebugEx;

namespace Module.Extension.System
{
    public class SimpleAgent : MonoBehaviour
    {
        [SerializeField] private Rigidbody rig;
        [SerializeField] private Vector3 gravity;
        [SerializeField] private float speed;
        [SerializeField] private float stoppingDistance = 2f;

        public void Move(Vector3 target)
        {
            if (rig.isKinematic)
            {
                return;
            }

            Vector3 distance = target - transform.position;
            Vector3 normal = distance.normalized;

            if (distance.sqrMagnitude <= stoppingDistance * stoppingDistance)
            {
                rig.velocity = Vector3.zero;
                rig.position = target + (-normal * stoppingDistance);
            }
            else
            {
                rig.velocity = normal * speed + gravity;
            }

            LookAt(normal);
        }

        private void LookAt(Vector3 direction)
        {
            direction.y = 0f;
            Quaternion lookRotation = Quaternion.LookRotation(direction, Vector3.up);
            transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, 0.1f);
        }

        public void SetActive(bool isActive)
        {
            rig.isKinematic = !isActive;
        }
    }
}