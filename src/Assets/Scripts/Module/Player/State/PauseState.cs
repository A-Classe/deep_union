using UnityEngine;

namespace Module.Player.State
{
    public class PauseState : IPlayerState
    {
        private readonly Rigidbody rigidbody;
        private Vector3 velocity;
        private Vector3 angularVelocity;

        public PauseState(Rigidbody rigidbody)
        {
            this.rigidbody = rigidbody;
        }

        public PlayerState GetState()
        {
            return PlayerState.Pause;
        }

        public void Start()
        {
            velocity = rigidbody.velocity;
            angularVelocity = rigidbody.angularVelocity;
            rigidbody.isKinematic = true;
        }

        public void Stop()
        {
            rigidbody.velocity = velocity;
            rigidbody.angularVelocity = angularVelocity;
            rigidbody.isKinematic = false;
        }

        public void FixedUpdate() { }
    }
}