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

            if (distance.sqrMagnitude <= stoppingDistance * stoppingDistance)
            {
                rig.velocity = Vector3.zero;
            }
            else
            {
                rig.velocity = distance.normalized * speed + gravity;
            }

            transform.LookAt(target);
        }

        public void SetActive(bool isActive)
        {
            rig.isKinematic = !isActive;
        }
    }
}