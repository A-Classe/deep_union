using System;
using Core.Input;
using UnityEngine;

namespace Module.Working.Controller
{
    /// <summary>
    ///     群体を操作するクラス
    /// </summary>
    public class WorkerController : MonoBehaviour
    {
        [Header("移動速度")] [SerializeField] private float controlSpeed;
        [Header("最大速度")] [SerializeField] private float maxSpeed;

        [SerializeField] private Transform target;
        [SerializeField] private Rigidbody rig;
        [SerializeField] private bool isUpdatePlayerOffset;
        private float beforeZ;

        private InputEvent controlEvent;

        private Vector2 input;

        
        private Vector3 lastPosition = Vector3.zero;
        
        public event Action<float> OnMoveDistance; 

        private void Awake()
        {
            //入力イベントの生成
            controlEvent = InputActionProvider.Instance.CreateEvent(ActionGuid.InGame.Control);
            beforeZ = target.position.z;
        }

        private void Update()
        {
            input = controlEvent.ReadValue<Vector2>();
        }

        private void FixedUpdate()
        {
            var velocity = rig.velocity;

            if (input != Vector2.zero)
            {
                var forward = target.forward * input.y;
                var right = target.right * input.x;
                var controlDir = (forward + right).normalized;
                var vel = controlDir * (controlSpeed * Time.fixedDeltaTime);
                velocity += new Vector3(vel.x, 0, vel.z);
            }

            rig.velocity = Vector3.ClampMagnitude(velocity, maxSpeed);
            rig.position += velocity * Time.fixedDeltaTime;

            if (isUpdatePlayerOffset)
            {
                UpdatePlayerOffset();
            }
            SendLog();
        }

        private void UpdatePlayerOffset()
        {
            rig.position += new Vector3(0f, 0f, target.position.z - beforeZ);

            beforeZ = target.position.z;
        }

        private void SendLog()
        {
            if (lastPosition != Vector3.zero)
            {
                float distance = Vector3.Distance(lastPosition, transform.position);
                if (Math.Abs(distance) < 0.001f) return;
                OnMoveDistance?.Invoke(distance);
            }
            lastPosition = transform.position;
        }
    }
}