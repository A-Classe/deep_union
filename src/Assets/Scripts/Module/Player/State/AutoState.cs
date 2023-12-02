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
            currentSpeed += movementSetting.Acceleralation * Time.fixedDeltaTime;
            currentSpeed = Mathf.Clamp(currentSpeed, movementSetting.MinSpeed, movementSetting.MaxSpeed);

            rigidbody.velocity = rigidbody.transform.forward * currentSpeed;
        }
    }
}