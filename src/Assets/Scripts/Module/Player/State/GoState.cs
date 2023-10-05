using Module.Player.Controller;
using UnityEngine;

namespace Module.Player.State
{
    public class GoState : IPlayerState
    {
        private readonly PlayerController controller;
        public GoState(PlayerController controller)
        {
            this.controller = controller;
        }
        public PlayerState GetState() => PlayerState.Go;

        public void Update()
        {
            var transform = controller.transform;
            var position = transform.position;
            var updatePosition = Vector3.Lerp(position, position + transform.forward * controller.Speed, Time.deltaTime);
            controller.transform.position = updatePosition;
        }
    }
}