using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VContainer;

namespace Module.Task
{
    /// <summary>
    ///     全てのタスクのベースクラス
    /// </summary>
    [DisallowMultipleComponent]
    public abstract class BaseTask : MonoBehaviour, ITaskSystem
    {
        [SerializeField] private int mw;

        private List<Collider> taskColliders;

        [SerializeField] protected TaskState state = TaskState.Idle;

        [SerializeField] private float currentProgress;
        [SerializeField] private int currentWorkerCount;
        private float prevWorkerCount;

        public float Progress => currentProgress;
        public TaskState State => state;
        public int MonoWork => mw;

        public event Action<float> OnProgressChanged;
        public event Action<BaseTask> OnCompleted;

        /// <summary>
        ///     現在割り当てられているワーカー数
        /// </summary>
        public int WorkerCount
        {
            set
            {
                prevWorkerCount = currentWorkerCount;
                currentWorkerCount = value;
                OnMonoWorkUpdated();
            }
            get => currentWorkerCount;
        }

        private void Awake()
        {
            taskColliders = GetComponentsInChildren<Collider>().ToList();
            taskColliders.AddRange(GetComponents<Collider>());
        }

        /// <summary>
        ///     ゲーム開始時の初期化関数
        /// </summary>
        /// <param name="container"></param>
        public virtual void Initialize(IObjectResolver container) { }

        /// <summary>
        ///     Taskの状態を更新するUpdate
        /// </summary>
        /// <param name="deltaTime">Time.deltaTime</param>
        void ITaskSystem.TaskSystemUpdate(float deltaTime)
        {
            UpdateProgress(deltaTime);
            ManagedUpdate(deltaTime);
        }

        public event Action<TaskState> OnStateChanged;

        /// <summary>
        ///     ゲームの状態によって管理されるUpdate
        /// </summary>
        /// <param name="deltaTime">Time.deltaTime</param>
        protected virtual void ManagedUpdate(float deltaTime) { }

        private void UpdateProgress(float deltaTime)
        {
            if (state == TaskState.Completed || currentWorkerCount == 0f)
                return;

            float currentMw = Mathf.Clamp(mw * currentProgress + currentWorkerCount * deltaTime, 0f, mw);
            float prevProgress = currentProgress;
            currentProgress = Mathf.InverseLerp(0f, mw, currentMw);

            if (currentProgress - prevProgress != 0)
            {
                OnProgressChanged?.Invoke(currentProgress);
            }

            //進捗が1に到達したら完了
            if (currentProgress >= 1f)
            {
                ForceComplete();
            }
        }

        private void OnMonoWorkUpdated()
        {
            if (state == TaskState.Completed)
                return;

            //作業量が0より大きくなったら開始
            if (prevWorkerCount == 0f && currentWorkerCount > prevWorkerCount)
            {
                ChangeState(TaskState.InProgress);
                OnStart();
            }
            //作業量が0になったらキャンセル
            else if (prevWorkerCount > 0f && currentWorkerCount == 0f)
            {
                ChangeState(TaskState.Idle);
                OnCancel();
            }
        }

        // ReSharper disable once ParameterHidesMember
        private void ChangeState(TaskState state)
        {
            this.state = state;
            OnStateChanged?.Invoke(state);
        }

        protected void ForceComplete()
        {
            currentProgress = 1f;
            ChangeState(TaskState.Completed);
            OnComplete();
            OnCompleted?.Invoke(this);
        }

        protected virtual void OnStart() { }

        protected virtual void OnCancel() { }

        protected virtual void OnComplete() { }

        public void SetDetection(bool isEnabled)
        {
            foreach (Collider col in taskColliders)
            {
                col.enabled = isEnabled;
            }
        }

        public void Enable()
        {
            gameObject.SetActive(true);
        }

        public void Disable()
        {
            gameObject.SetActive(false);
        }
    }
}