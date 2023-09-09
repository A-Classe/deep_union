using System;
using System.Collections.Generic;
using Core.Input;
using GameMain.Presenter;
using Module.Task;
using Module.Working;
using Module.Working.Controller;
using Module.Working.State;
using UnityEngine;
using VContainer;

namespace Module.Assignment
{
    public class LeaderAssignEvent : MonoBehaviour
    {
        [SerializeField] private AssignableArea assignableArea;
        [SerializeField] private GameParam gameParam;

        private List<Worker> workers;
        public int WorkerCount => workers.Count;
        private bool isWorldMoving;
        private InputEvent assignEvent;
        private InputEvent releaseEvent;
        private float defaultIntensity;

        private void Awake()
        {
            workers = new List<Worker>();

            assignableArea.OnWorkerEnter += AddWorker;
            assignableArea.OnWorkerExit += RemoveWorker;

            assignEvent = InputActionProvider.Instance.CreateEvent(ActionGuid.InGame.Assign);
            releaseEvent = InputActionProvider.Instance.CreateEvent(ActionGuid.InGame.Release);

            defaultIntensity = assignableArea.Intensity;

            assignEvent.Started += ctx =>
            {
                assignableArea.SetLightIntensity(gameParam.AssignIntensity);
            };

            assignEvent.Canceled += ctx =>
            {
                assignableArea.SetLightIntensity(defaultIntensity);
            };

            releaseEvent.Started += ctx =>
            {
                assignableArea.SetLightIntensity(gameParam.ReleaseIntensity);
            };

            releaseEvent.Canceled += ctx =>
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
            workers.Add(worker);
            worker.IsWorldMoving = isWorldMoving;
        }

        private void RemoveWorker(Worker worker)
        {
            workers.Remove(worker);
            worker.IsWorldMoving = false;
        }

        public void SetWorldMovingActive(bool enable)
        {
            isWorldMoving = enable;

            foreach (Worker worker in workers)
            {
                worker.IsWorldMoving = enable;
            }
        }
    }
}