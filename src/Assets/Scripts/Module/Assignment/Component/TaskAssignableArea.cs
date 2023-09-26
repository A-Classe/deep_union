using Module.Task;
using Module.Working;
using Module.Working.State;
using UnityEngine;

namespace Module.Assignment.Component
{
    /// <summary>
    /// タスクのアサイン機能を拡張するクラス
    /// </summary>
    public class TaskAssignableArea : MonoBehaviour
    {
        [SerializeField] private AssignableArea assignableArea;

        private BaseTask baseTask;

        private void Start()
        {
            baseTask = transform.parent.GetComponent<BaseTask>();

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