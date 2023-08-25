using Module.Player.Controller;
using UnityEngine;
using Wanna.DebugEx;

namespace Module.Player.State
{
    public class GoState : IPlayerState
    {
        private PlayerController controller;
        public GoState(PlayerController controller)
        {
            this.controller = controller;
        }
        public PlayerState GetState() => PlayerState.Go;

        public void Update()
        {
            var transform = controller.transform;
            Vector3 position = transform.position;
            Vector3 updatePosition = Vector3.Lerp(position, position + transform.forward * controller.Speed, Time.deltaTime);
            DebugEx.Log(updatePosition);
            controller.transform.position = updatePosition;
        }
    }
}