using Core.Input;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Controller
{
    /// <summary>
    /// 群体を操作するクラス
    /// </summary>
    public class WorkerController : MonoBehaviour
    {
        [Header("移動速度")] [SerializeField] private float controlSpeed;

        private InputEvent controlEvent;
        private InputEvent assignEvent;
        private Rigidbody rb = default;

        /// <summary>
        /// リリース状態か
        /// </summary>
        public bool DoRelease { get; private set; }

        private void Start()
        {
            rb = GetComponent<Rigidbody>();

            //入力イベントの生成
            controlEvent = InputActionProvider.Instance.CreateEvent(ActionGuid.InGame.Control);
            assignEvent = InputActionProvider.Instance.CreateEvent(ActionGuid.InGame.Assign);

            assignEvent.Started += Release;
            assignEvent.Canceled += Release;
        }

        private void FixedUpdate()
        {
            //速度の更新
            UpdateVelocity();
        }

        public Vector3 GetPosition()
        {
            return transform.position;
        }

        private void Release(InputAction.CallbackContext ctx)
        {
            DoRelease = ctx.started;
        }

        private void UpdateVelocity()
        {
            Vector2 input = controlEvent.ReadValue<Vector2>();

            Vector3 velocity = rb.velocity;
            velocity.x += input.x * controlSpeed;
            velocity.z += input.y * controlSpeed;

            rb.velocity = velocity;
        }
    }
}