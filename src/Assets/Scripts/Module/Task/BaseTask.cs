using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

namespace Module.Task
{
    /// <summary>
    ///     全てのタスクのベースクラス
    /// </summary>
    [RequireComponent(typeof(SphereCollider))]
    [DisallowMultipleComponent]
    public abstract class BaseTask : MonoBehaviour, ITaskSystem
    {
        [Header("検出されるタスクの半径")]
        [SerializeField]
        [Range(0f, 6f)]
        private float taskSize = 1f;

        [Header("タスクのMonoWork")]
        [SerializeField]
        private float mw;

        [Header("アサイン可能な座標\n(配列のサイズがタスクのキャパになります)")]
        [SerializeField]
        private List<Transform> assignPoints;

        [Header("!デバッグ用 書き換え禁止!")]
        [SerializeField]
        protected TaskState state = TaskState.Idle;

        [SerializeField] [Range(0f, 1f)] private float currentProgress;
        [SerializeField] private float currentMw;
        [SerializeField] private float progressMw;
        private float prevMw;

        /// <summary>
        ///     現在割り当てられている作業量
        /// </summary>
        public float Mw
        {
            set
            {
                prevMw = currentMw;
                currentMw = Mathf.Clamp(value, 0f, mw);
                OnMonoWorkUpdated();
            }
            get => currentMw;
        }

        // ReSharper disable once UnusedMember.Global
        public float Progress => currentProgress;

        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(0f, 0.83f, 0f, 0.41f);
            Gizmos.DrawSphere(transform.position, taskSize);
        }

        private void OnValidate()
        {
            var col = GetComponent<SphereCollider>();
            col.radius = taskSize;

            //作業量は最低1以上とする
            mw = Mathf.Clamp(mw, 1f, float.MaxValue);
        }

        public TaskState State => state;

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

        public Transform GetNearestAssignPoint(Vector3 target)
        {
            assignPoints.Sort();
        }

        private void UpdateProgress(float deltaTime)
        {
            if (state == TaskState.Completed || currentMw == 0f)
                return;

            progressMw = Mathf.Clamp(progressMw + currentMw * deltaTime, 0f, mw);
            currentProgress = Mathf.InverseLerp(0f, mw, progressMw);

            //進捗が1に到達したら完了
            if (currentProgress >= 1f)
            {
                ChangeState(TaskState.Completed);
                OnComplete();
            }
        }

        private void OnMonoWorkUpdated()
        {
            if (state == TaskState.Completed)
                return;

            //作業量が0より大きくなったら開始
            if (prevMw == 0f && currentMw > prevMw)
            {
                ChangeState(TaskState.InProgress);
                OnStart();
            }
            //作業量が0になったらキャンセル
            else if (prevMw > 0f && currentMw == 0f)
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

        protected virtual void OnStart() { }

        protected virtual void OnCancel() { }

        protected virtual void OnComplete() { }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;

            foreach (Transform point in assignPoints)
            {
                if (point == null)
                    return;

                Gizmos.DrawSphere(point.position, 0.3f);
            }
        }
    }
}