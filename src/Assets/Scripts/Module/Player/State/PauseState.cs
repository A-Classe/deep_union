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

        public PlayerState GetState() => PlayerState.Pause;

        public void Update() { }
    }
}