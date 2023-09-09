using System;
using Module.Task;
using Module.Working;
using Module.Working.State;
using UnityEngine;

namespace Module.Assignment
{
    public class TaskAssignEvent : MonoBehaviour
    {
        [SerializeField] private BaseTask baseTask;
        [SerializeField] private AssignableArea assignableArea;

        private void Start()
        {
            assignableArea.OnWorkerEnter += AddWorker;
            assignableArea.OnWorkerExit += RemoveWorker;
        }

        private void AddWorker(Worker worker)
        {
            worker.SetWorkerState(WorkerState.Working);
            baseTask.WorkerCount += 1;
        }

        private void RemoveWorker(Worker worker)
        {
            baseTask.WorkerCount -= 1;
        }
    }
}