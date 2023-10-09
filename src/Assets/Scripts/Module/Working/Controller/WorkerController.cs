using System;
using Core.Input;
using UnityEngine;
using Wanna.DebugEx;

namespace Module.Working.Controller
{
    /// <summary>
    ///     群体を操作するクラス
    /// </summary>
    public class WorkerController : MonoBehaviour
    {
        [Header("移動速度")] [SerializeField] private float controlSpeed;

        [Header("静的摩擦力")] [SerializeField] private float staticFriction;
        [Header("最大速度")] [SerializeField] private float maxSpeed;

        [SerializeField] private Vector3 velocity;

        private InputEvent controlEvent;

        private Camera followCamera;
        private Vector2 input;

        private void Awake()
        {
            //入力イベントの生成
            controlEvent = InputActionProvider.Instance.CreateEvent(ActionGuid.InGame.Control);
        }

        private void Update()
        {
            input = controlEvent.ReadValue<Vector2>();
        }

        private void FixedUpdate()
        {
            if (input != Vector2.zero)
            {
                Vector2 vel = input * (controlSpeed * Time.fixedDeltaTime);
                velocity += new Vector3(vel.x, 0, vel.y);
            }
            else
            {
                float friction = staticFriction * Time.fixedDeltaTime;
                float frictionMagnitude = velocity.magnitude - friction;

                velocity = velocity.normalized * frictionMagnitude;
            }

            velocity = Vector3.ClampMagnitude(velocity, maxSpeed);

            Vector3 nextPos = transform.position + velocity * Time.fixedDeltaTime;

            if (!InViewport(nextPos))
            {
                velocity = Vector3.zero;   
            }

            transform.position = Clamp(nextPos);
        }

        private bool InViewport(Vector3 worldPoint)
        {
            Vector3 inViewport = followCamera.WorldToViewportPoint(worldPoint);

            return (inViewport.x is > 0 and < 1 &&
                    inViewport.y is > 0 and < 1 &&
                    inViewport.z > 0);
        }


        private Vector3 Clamp(Vector3 nextPosition)
        {
            Vector3 viewportPoint = followCamera.WorldToViewportPoint(nextPosition);

            // オブジェクトの位置が画面外に出ないようにクランプ
            viewportPoint.x = Mathf.Clamp01(viewportPoint.x);
            viewportPoint.y = Mathf.Clamp01(viewportPoint.y);

            Vector3 worldPoint = followCamera.ViewportToWorldPoint(viewportPoint);
            worldPoint.y = nextPosition.y;

            return worldPoint;
        }

        public void SetCamera(Camera cam)
        {
            followCamera = cam;
        }
    }
}