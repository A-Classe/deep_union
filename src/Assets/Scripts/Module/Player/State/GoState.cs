using System;
using Core.Input;
using GameMain.Presenter;
using Module.Player.Controller;
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
    
    public class GoState : IPlayerState
    {
        private readonly PlayerController controller;
        private readonly GameParam gameParam;
        private readonly InputEvent moveInput;
        private readonly Rigidbody rigidbody;
        private readonly MovementSetting movementSetting;
        private readonly InputEvent rotateInput;
        private float currentRotation;
        private float currentSpeed;

        private float rotationSpeed;

        public GoState(PlayerController controller, Rigidbody rigidbody, MovementSetting movementSetting)
        {
            this.controller = controller;
            gameParam = controller.gameParam;
            this.rigidbody = rigidbody;
            this.movementSetting = movementSetting;

            moveInput = InputActionProvider.Instance.CreateEvent(ActionGuid.InGame.Move);
            rotateInput = InputActionProvider.Instance.CreateEvent(ActionGuid.InGame.Rotate);
        }

        public PlayerState GetState()
        {
            return PlayerState.Go;
        }

        public void Update()
        {
            var inputY = 1f;

            var transform = controller.transform;
            var forward = transform.forward;

            if (inputY != 0f)
            {
                currentSpeed += inputY * movementSetting.Acceleralation * Time.fixedDeltaTime;
            }
            else
            {
                currentSpeed -= movementSetting.Resistance * Time.fixedDeltaTime;
            }

            currentSpeed = Mathf.Clamp(currentSpeed, movementSetting.MinSpeed, movementSetting.MaxSpeed);
            rigidbody.velocity = forward * currentSpeed;

            // Rigidbodyを使用してオブジェクトを回転
            var rotation = new Vector3(0, currentRotation, 0);
            rigidbody.MoveRotation(Quaternion.Euler(rotation));
        }
    }
}