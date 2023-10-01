using System;
using UnityEngine;
using Wanna.DebugEx;

namespace GameMain.System
{
    public class SimpleAgent : MonoBehaviour
    {
        [SerializeField] private Rigidbody rig;
        [SerializeField] private float speed;

        public void Move(Vector3 target)
        {
            rig.velocity = (target - transform.position).normalized * speed;
            transform.LookAt(target);
        }

        public void SetActive(bool isActive)
        {
            rig.isKinematic = !isActive;
        }
    }
}