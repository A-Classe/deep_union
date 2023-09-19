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

        private InputEvent controlEvent;

        private Camera followCamera;

        private void Awake()
        {
            //入力イベントの生成
            controlEvent = InputActionProvider.Instance.CreateEvent(ActionGuid.InGame.Control);
        }

        private void Update()
        {
            //リーダーの速度の更新
            UpdateLeaderVelocity();
        }

        private void UpdateLeaderVelocity()
        {
            var input = controlEvent.ReadValue<Vector2>();

            float moveX = input.x * controlSpeed * Time.deltaTime;
            float moveY = input.y * controlSpeed * Time.deltaTime;
            
            Vector3 move = new Vector3(moveX, 0f, moveY);

            Vector3 position = transform.position + move;
            if (!InViewport(position)) return;

            transform.position = position;
        }

        private bool InViewport(Vector3 position)
        {
            if (followCamera != null)
            {
                Vector3 inViewport = followCamera.WorldToViewportPoint(position);

                return (inViewport.x is > 0 and < 1 &&
                        inViewport.y is > 0 and < 1 &&
                        inViewport.z > 0);
            }

            return false;
        }

        public void SetCamera(Camera cam)
        {
            followCamera = cam;
        }
    }
}