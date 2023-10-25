using Core.Input;
using GameMain.Presenter;
using Module.Player.Controller;
using UnityEngine;
using Wanna.DebugEx;

namespace Module.Player.State
{
    public class GoState : IPlayerState
    {
        private readonly PlayerController controller;
        private readonly GameParam gameParam;
        private readonly Rigidbody rigidbody;
        private readonly InputEvent moveInput;
        private readonly InputEvent rotateInput;

        private float rotationSpeed;
        private float currentSpeed;
        private float currentRotation;

        public GoState(PlayerController controller, Rigidbody rigidbody)
        {
            this.controller = controller;
            this.gameParam = controller.gameParam;
            this.rigidbody = rigidbody;

            moveInput = InputActionProvider.Instance.CreateEvent(ActionGuid.InGame.Move);
            rotateInput = InputActionProvider.Instance.CreateEvent(ActionGuid.InGame.Rotate);
        }

        public PlayerState GetState() => PlayerState.Go;

        public void Update()
        {
            float inputX = rotateInput.ReadValue<float>();
            float inputY = moveInput.ReadValue<float>();

            var transform = controller.transform;
            var forward = transform.forward;

            currentSpeed += gameParam.MoveAccelaration * inputY;
            currentSpeed = Mathf.Clamp(currentSpeed, gameParam.MinSpeed, gameParam.MaxSpeed);

            rigidbody.velocity = forward * currentSpeed;

            // 回転速度を増加
            rotationSpeed += inputX * gameParam.TorqueAccelaration * Time.deltaTime;

            // 現在の回転を更新
            currentRotation += rotationSpeed * Time.deltaTime;

            if (currentRotation < -gameParam.AngleLimit || gameParam.AngleLimit < currentRotation)
            {
                rotationSpeed = 0f;
                currentRotation = currentRotation > gameParam.AngleLimit ? gameParam.AngleLimit : -gameParam.AngleLimit;
            }

            // Rigidbodyを使用してオブジェクトを回転
            Vector3 rotation = new Vector3(0, currentRotation, 0);
            rigidbody.MoveRotation(Quaternion.Euler(rotation));
        }
    }
}