using System;
using UnityEngine;

namespace Module.Player.State
{
    [Serializable]
    public class MovementSetting
    {
        public float MinSpeed;
        public float MaxSpeed;
        public float Acceleralation;
        public float Resistance;
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
        }
    }
}