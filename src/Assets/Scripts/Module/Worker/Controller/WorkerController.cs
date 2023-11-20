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
        private float beforeZ;

        private InputEvent controlEvent;

        private Camera followCamera;
        private Vector2 input;

        private bool isPlaying;

        private void Awake()
        {
            //入力イベントの生成
            controlEvent = InputActionProvider.Instance.CreateEvent(ActionGuid.InGame.Control);
            isPlaying = true;
            beforeZ = target.position.z;
        }

        private void Update()
        {
            input = controlEvent.ReadValue<Vector2>();
        }

        private void FixedUpdate()
        {
            if (!isPlaying) return;

            var velocity = rig.velocity;

            if (input != Vector2.zero)
            {
                var forward = target.forward * input.y;
                var right = target.right * input.x;
                var dir = (forward + right).normalized;
                var vel = dir * (controlSpeed * Time.fixedDeltaTime);
                velocity += new Vector3(vel.x, 0, vel.z);
            }

            rig.velocity = Vector3.ClampMagnitude(velocity, maxSpeed);

            var nextPos = rig.position + velocity * Time.fixedDeltaTime;

            if (!InViewport(nextPos)) rig.velocity = Vector3.zero;

            rig.position = Clamp(nextPos);

            UpdatePlayerOffset();
        }

        private void UpdatePlayerOffset()
        {
            rig.position += new Vector3(0f, 0f, target.position.z - beforeZ);

            beforeZ = target.position.z;
        }

        private bool InViewport(Vector3 worldPoint)
        {
            var inViewport = followCamera.WorldToViewportPoint(worldPoint);

            return inViewport.x is > 0 and < 1 &&
                   inViewport.y is > 0 and < 1 &&
                   inViewport.z > 0;
        }


        private Vector3 Clamp(Vector3 nextPosition)
        {
            var viewportPoint = followCamera.WorldToViewportPoint(nextPosition);

            // オブジェクトの位置が画面外に出ないようにクランプ
            viewportPoint.x = Mathf.Clamp01(viewportPoint.x);
            viewportPoint.y = Mathf.Clamp01(viewportPoint.y);

            var worldPoint = followCamera.ViewportToWorldPoint(viewportPoint);
            worldPoint.y = nextPosition.y;

            return worldPoint;
        }

        public void SetCamera(Camera cam)
        {
            followCamera = cam;
        }

        public void SetPlayed(bool value)
        {
            isPlaying = value;
        }
    }
}