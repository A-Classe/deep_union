using System.Collections.Generic;
using System.Linq;
using Core.Input;
using Module.Working.State;
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
        private Rigidbody leaderRb;
        private List<Worker> workers;

        private void Awake()
        {
            leaderRb = GetComponent<Rigidbody>();
            workers = new List<Worker>();

            //入力イベントの生成
            controlEvent = InputActionProvider.Instance.CreateEvent(ActionGuid.InGame.Control);
        }

        private void FixedUpdate()
        {
            //リーダーの速度の更新
            UpdateLeaderVelocity();
        }

        public void EnqueueWorker(Worker worker)
        {
            workers.Add(worker);

            worker.SetFollowTarget(transform);
            worker.SetWorkerState(WorkerState.Following);
        }

        public Worker DequeueNearestWorker(Vector3 position)
        {
            if (workers.Count == 0)
                return null;

            var worker = workers.OrderBy(worker => (position - worker.transform.position).sqrMagnitude).First();
            var index = workers.IndexOf(worker);
            workers.RemoveAt(index);

            return worker;
        }

        private void UpdateLeaderVelocity()
        {
            var input = controlEvent.ReadValue<Vector2>();

            var velocity = leaderRb.velocity;
            velocity.x += input.x * controlSpeed;
            velocity.z += input.y * controlSpeed;

            leaderRb.velocity = velocity;
        }
    }
}