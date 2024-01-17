using System;
using Core.Input;
using GameMain.Presenter;
using Module.Working;
using Module.Working.State;
using UnityEngine;

namespace Module.Assignment.Component
{
    /// <summary>
    ///     リーダーのアサイン機能を拡張するクラス
    /// </summary>
    public class LeaderAssignableArea : MonoBehaviour
    {
        [SerializeField] private AssignableArea assignableArea;
        [SerializeField] private AssignableAreaLight areaLight;
        [SerializeField] private GameParam gameParam;
        private InputEvent assignEvent;
        private float defaultIntensity;
        private bool isWorldMoving;
        private InputEvent releaseEvent;

        public int WorkerCount => assignableArea.AssignedWorkers.Count;

        public AssignableArea AssignableArea => assignableArea;

        public event Action<AssignableArea.WorkerEventType> OnChangedWorkers;

        private void Awake()
        {
            assignableArea.OnWorkerEnter += AddWorker;
            assignableArea.OnWorkerExit += RemoveWorker;

            assignEvent = InputActionProvider.Instance.CreateEvent(ActionGuid.InGame.Assign);
            releaseEvent = InputActionProvider.Instance.CreateEvent(ActionGuid.InGame.Release);

            defaultIntensity = areaLight.Intensity;

            assignEvent.Started += _ =>
            {
                areaLight.SetIntensity(gameParam.AssignIntensity);
            };

            assignEvent.Canceled += _ =>
            {
                areaLight.SetIntensity(defaultIntensity);
            };

            releaseEvent.Started += _ =>
            {
                areaLight.SetIntensity(gameParam.ReleaseIntensity);
            };

            releaseEvent.Canceled += _ =>
            {
                areaLight.SetIntensity(defaultIntensity);
            };
        }

        /// <summary>
        ///     ワーカーの目標をリーダーに設定します
        /// </summary>
        /// <param name="worker">設定するワーカー</param>
        private void AddWorker(Worker worker, AssignableArea.WorkerEventType type)
        {
            OnChangedWorkers?.Invoke(type);
            worker.SetWorkerState(WorkerState.Following);
            worker.IsWorldMoving = isWorldMoving;
        }

        private void RemoveWorker(Worker worker, AssignableArea.WorkerEventType type)
        {
            OnChangedWorkers?.Invoke(type);
            worker.IsWorldMoving = false;
        }

        public void SetWorldMovingActive(bool enable)
        {
            isWorldMoving = enable;

            foreach (var worker in assignableArea.AssignedWorkers)
            {
                worker.IsWorldMoving = enable;
            }
        }
    }
}