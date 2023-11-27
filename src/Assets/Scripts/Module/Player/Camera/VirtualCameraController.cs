using Core.Input;
using UnityEngine;

namespace Module.Player.Camera
{
    public class VirtualCameraController : MonoBehaviour
    {
        [SerializeField] private Transform followTarget;
        [SerializeField] private float speed;
        [SerializeField] private float dampingSpeed;
        [SerializeField] private float maxSpeed;

        private InputEvent rotateEvent;
        private bool isRotating;
        private float angularVelocity;

        private void Start()
        {
            rotateEvent = InputActionProvider.Instance.CreateEvent(ActionGuid.InGame.RotateCamera);
        }

        private void FixedUpdate()
        {
            float delta = rotateEvent.ReadValue<float>() * speed;

            if (delta != 0f)
            {
                isRotating = true;
                
                //加速する
                angularVelocity += delta * speed * Time.fixedDeltaTime;
                
                //最大速度にクランプ
                angularVelocity = Mathf.Clamp(angularVelocity, -maxSpeed, maxSpeed);
            }
            else if (isRotating)
            {
                //キーが押されていない間は減衰させる
                angularVelocity = Mathf.Lerp(angularVelocity, 0f, dampingSpeed * Time.fixedDeltaTime);

                //一定速度以下になったら回転終了
                if (Mathf.Abs(angularVelocity) < 0.1f)
                {
                    isRotating = false;
                    angularVelocity = 0f;
                }
            }

            followTarget.Rotate(Vector3.up, angularVelocity * Time.fixedDeltaTime);
        }
    }
}