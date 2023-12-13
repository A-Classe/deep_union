using UnityEngine;

namespace Module.Player.State
{
    public class StopState : IPlayerState
    {
        private readonly Rigidbody rigidbody;
        private readonly MovementSetting movementSetting;
        private float currentSpeed;

        public StopState(Rigidbody rigidbody, MovementSetting movementSetting)
        {
            this.rigidbody = rigidbody;
            this.movementSetting = movementSetting;
        }

        public PlayerState GetState()
        {
            return PlayerState.Stop;
        }

        public void Start()
        {
            currentSpeed = rigidbody.velocity.magnitude;
        }

        public void Stop() { }

        public void FixedUpdate()
        {
            //徐々に減速しながら停止
            currentSpeed -= movementSetting.Resistance * Time.fixedDeltaTime;

            //速度制限
            currentSpeed = Mathf.Clamp(currentSpeed, 0f, movementSetting.MaxSpeed);

            rigidbody.velocity = rigidbody.transform.forward * currentSpeed;
        }
    }
}