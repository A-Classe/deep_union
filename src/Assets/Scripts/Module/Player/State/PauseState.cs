using UnityEngine;

namespace Module.Player.State
{
    public class PauseState : IPlayerState
    {
        private readonly Rigidbody rigidbody;

        public PauseState(Rigidbody rigidbody)
        {
            this.rigidbody = rigidbody;
        }

        public PlayerState GetState()
        {
            return PlayerState.Pause;
        }

        public void Update()
        {
            if (rigidbody.velocity != Vector3.zero) rigidbody.velocity = Vector3.zero;
        }
    }
}