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
        public float AvoidSpeed;
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

            //正面にレイを飛ばして壁を判定
            if (Physics.Raycast(position, forward, out RaycastHit hitInfo, movementSetting.DetectDistance, movementSetting.DetectLayerMask.value))
            {
                //壁を避ける角速度を算出
                float angle = Vector3.Angle(forward, Vector3.Reflect(forward, hitInfo.normal)) * Time.fixedDeltaTime;
                rigidbody.angularVelocity = Vector3.up * (-angle * movementSetting.AvoidSpeed);
            }
        }
    }
}