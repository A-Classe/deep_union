using Core.Input;
using GameMain.Presenter;
using Module.Working;
using Module.Working.State;
using UnityEngine;
using UnityEngine.AI;

namespace Module.Assignment.Component
{
    /// <summary>
    /// リーダーのアサイン機能を拡張するクラス
    /// </summary>
    public class LeaderAssignableArea : MonoBehaviour
    {
        [SerializeField] private AssignableArea assignableArea;
        [SerializeField] private GameParam gameParam;

        public int WorkerCount => assignableArea.AssignedWorkers.Count;
        private InputEvent assignEvent;
        private InputEvent releaseEvent;
        private bool isWorldMoving;
        private float defaultIntensity;

        public AssignableArea AssignableArea => assignableArea;

        private void Awake()
        {
            assignableArea.OnWorkerEnter += AddWorker;
            assignableArea.OnWorkerExit += RemoveWorker;

            assignEvent = InputActionProvider.Instance.CreateEvent(ActionGuid.InGame.Assign);
            releaseEvent = InputActionProvider.Instance.CreateEvent(ActionGuid.InGame.Release);

            defaultIntensity = assignableArea.Intensity;

            assignEvent.Started += _ =>
            {
                assignableArea.SetLightIntensity(gameParam.AssignIntensity);
            };

            assignEvent.Canceled += _ =>
            {
                assignableArea.SetLightIntensity(defaultIntensity);
            };

            releaseEvent.Started += _ =>
            {
                assignableArea.SetLightIntensity(gameParam.ReleaseIntensity);
            };

            releaseEvent.Canceled += _ =>
            {
                assignableArea.SetLightIntensity(defaultIntensity);
            };
        }

        /// <summary>
        /// ワーカーの目標をリーダーに設定します
        /// </summary>
        /// <param name="worker">設定するワーカー</param>
        private void AddWorker(Worker worker)
        {
            worker.SetWorkerState(WorkerState.Following);
            worker.IsWorldMoving = isWorldMoving;
        }

        private void RemoveWorker(Worker worker)
        {
            worker.IsWorldMoving = false;
        }

        public void SetWorldMovingActive(bool enable)
        {
            isWorldMoving = enable;

            foreach (Worker worker in assignableArea.AssignedWorkers)
            {
                worker.IsWorldMoving = enable;
            }
        }
    }
}