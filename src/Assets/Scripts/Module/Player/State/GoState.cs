using Core.Input;
using GameMain.Presenter;
using Module.Player.Controller;
using UnityEngine;

namespace Module.Player.State
{
    public class GoState : IPlayerState
    {
        private readonly PlayerController controller;
        private readonly GameParam gameParam;
        private readonly InputEvent moveInput;
        private readonly MovementSetting movementSetting;
        private readonly Rigidbody rigidbody;
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
            var inputX = rotateInput.ReadValue<float>();
            var inputY = moveInput.ReadValue<float>();

            var transform = controller.transform;
            var forward = transform.forward;

            if (inputY != 0f)
                currentSpeed += inputY * gameParam.MoveAccelaration * Time.fixedDeltaTime;
            else
                currentSpeed -= movementSetting.MoveResistance * Time.fixedDeltaTime;

            currentSpeed = Mathf.Clamp(currentSpeed, gameParam.MinSpeed, gameParam.MaxSpeed);
            rigidbody.velocity = forward * currentSpeed;

            if (inputX != 0f)
                // 回転速度を増加
                rotationSpeed += inputX * gameParam.TorqueAccelaration * Time.fixedDeltaTime;
            else
                rotationSpeed -= Mathf.Sign(rotationSpeed) * movementSetting.RotateResistance * Time.fixedDeltaTime;

            rotationSpeed = Mathf.Clamp(rotationSpeed, gameParam.MinRotateSpeed, gameParam.MaxRotateSpeed);
            currentRotation += rotationSpeed;

            if (currentRotation < -gameParam.AngleLimit || gameParam.AngleLimit < currentRotation)
            {
                rotationSpeed = 0f;
                currentRotation = currentRotation > gameParam.AngleLimit ? gameParam.AngleLimit : -gameParam.AngleLimit;
            }

            // Rigidbodyを使用してオブジェクトを回転
            var rotation = new Vector3(0, currentRotation, 0);
            rigidbody.MoveRotation(Quaternion.Euler(rotation));
        }
    }
}