using System;
using UnityEngine;
using Wanna.DebugEx;
using Debug = System.Diagnostics.Debug;

namespace Module.Player.State
{
    [Serializable]
    public class MovementSetting
    {
        public float MinSpeed;
        public float MaxSpeed;
        public float Acceleralation;
        public float Resistance;

        [Header("回避設定")] public float DetectOffset = 1f;
        public float DetectDistance;
        public float AvoidRotation;
        public LayerMask DetectLayerMask;
    }

    /// <summary>
    /// 自動航行のステート
    /// </summary>
    public class AutoState : IPlayerState
    {
        private readonly Rigidbody rigidbody;
        private readonly MovementSetting movementSetting;
        private float currentSpeed;

        public AutoState(Rigidbody rigidbody, MovementSetting movementSetting)
        {
            this.rigidbody = rigidbody;
            this.movementSetting = movementSetting;
        }

        public PlayerState GetState()
        {
            return PlayerState.Auto;
        }

        public void Start()
        {
            currentSpeed = rigidbody.velocity.magnitude;
        }

        public void Stop() { }

        public void FixedUpdate()
        {
            //徐々に加速
            currentSpeed += movementSetting.Acceleralation * Time.fixedDeltaTime;

            //速度制限
            currentSpeed = Mathf.Clamp(currentSpeed, movementSetting.MinSpeed, movementSetting.MaxSpeed);

            rigidbody.velocity = rigidbody.transform.forward * currentSpeed;

            PerformAvoidance();
        }

        private void PerformAvoidance()
        {
            Vector3 position = rigidbody.position + new Vector3(0f, movementSetting.DetectOffset, 0f);
            Vector3 forward = rigidbody.transform.forward;

            DebugEx.DrawLine(position, position + forward * movementSetting.DetectDistance, Color.red, 0.5f);

            if (Physics.Raycast(position, forward, out RaycastHit hitInfo, movementSetting.DetectDistance, movementSetting.DetectLayerMask.value))
            {
                float t = -(Mathf.Sign(hitInfo.point.x) * (1 - Mathf.Abs(hitInfo.point.x)));
                DebugEx.Log(Vector3.up * (movementSetting.AvoidRotation * t));
                rigidbody.angularVelocity = Vector3.up * (movementSetting.AvoidRotation * t);
            }
        }
    }
}