using System.Collections.Generic;
using Core.Input;
using Module.Worker.State;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Module.Worker.Controller
{
    /// <summary>
    /// 群体を操作するクラス
    /// </summary>
    public class WorkerController : MonoBehaviour
    {
        [Header("移動速度")] [SerializeField] private float controlSpeed;

        private InputEvent controlEvent;
        private InputEvent assignEvent;
        private Rigidbody leaderRb = default;
        private List<Worker> workers;

        /// <summary>
        /// リリース状態か
        /// </summary>
        public bool DoRelease { get; private set; }

        private void Awake()
        {
            leaderRb = GetComponent<Rigidbody>();
            workers = new List<Worker>();

            //入力イベントの生成
            controlEvent = InputActionProvider.Instance.CreateEvent(ActionGuid.InGame.Control);
            assignEvent = InputActionProvider.Instance.CreateEvent(ActionGuid.InGame.Assign);

            assignEvent.Started += Release;
            assignEvent.Canceled += Release;
        }

        public void SetWorkers(IEnumerable<Worker> workers)
        {
            this.workers.AddRange(workers);

            foreach (Worker worker in this.workers)
            {
                worker.SetWorkerState(WorkerState.Following);
            }
        }

        private void FixedUpdate()
        {
            //リーダーの速度の更新
            UpdateLeaderVelocity();

            //ワーカーのターゲット更新
            UpdateWorkersFollowPoint();
        }

        private void Release(InputAction.CallbackContext ctx)
        {
            DoRelease = ctx.started;
        }

        private void UpdateLeaderVelocity()
        {
            Vector2 input = controlEvent.ReadValue<Vector2>();

            Vector3 velocity = leaderRb.velocity;
            velocity.x += input.x * controlSpeed;
            velocity.z += input.y * controlSpeed;

            leaderRb.velocity = velocity;
        }

        private void UpdateWorkersFollowPoint()
        {
            foreach (Worker worker in workers)
            {
                worker.SetFollowPoint(leaderRb.position);
            }
        }
    }
}