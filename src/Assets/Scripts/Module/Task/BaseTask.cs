using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
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
        [SerializeField] private float taskSize;
        [SerializeField] private float mw;
        [SerializeField] private bool debugAssignPoints;

        private List<AssignPoint> assignPoints;

        [SerializeField] protected TaskState state = TaskState.Idle;

        [SerializeField] private float currentProgress;
        [SerializeField] private int currentWorkerCount;
        private float prevWorkerCount;

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

        // ReSharper disable once UnusedMember.Global
        public float Progress => currentProgress;
        public TaskState State => state;

        private void Awake()
        {
            assignPoints = GetComponentsInChildren<AssignPoint>().ToList();
        }

        private void OnValidate()
        {
            var col = GetComponent<SphereCollider>();
            col.radius = taskSize;
            
            SetEnableAssignPointDebug(debugAssignPoints);
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

        public bool TryGetNearestAssignPoint(Vector3 target, out Transform assignPoint)
        {
            //アサインできる座標がなかったらアサイン不可
            if (assignPoints.Count == 0)
            {
                assignPoint = null;
                return false;
            }

            assignPoints.Sort((a, b) =>
                {
                    Vector3 p1 = target - a.transform.position;
                    Vector3 p2 = target - b.transform.position;

                    if (p1.sqrMagnitude - p2.sqrMagnitude > 0)
                    {
                        return 1;
                    }

                    return -1;
                }
            );

            assignPoint = assignPoints[0].transform;
            assignPoints.RemoveAt(0);

            return true;
        }

        public void ReleaseAssignPoint(Transform assignPoint)
        {
            assignPoints.Add(assignPoint.GetComponent<AssignPoint>());
        }

        private void UpdateProgress(float deltaTime)
        {
            if (state == TaskState.Completed || currentWorkerCount == 0f)
                return;

            float currentMw = Mathf.Clamp(mw * currentProgress + currentWorkerCount * deltaTime, 0f, mw);
            currentProgress = Mathf.InverseLerp(0f, mw, currentMw);

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

        protected virtual void OnStart() { }

        protected virtual void OnCancel() { }

        protected virtual void OnComplete() { }

        public void SetEnableAssignPointDebug(bool enable)
        {
            assignPoints = GetComponentsInChildren<AssignPoint>().ToList();

            //アサインポイントのデブッグを有効化する
            foreach (AssignPoint assignPoint in assignPoints)
            {
                if (assignPoint == null)
                    return;

                assignPoint.enabled = enable;
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(0f, 0.83f, 0f, 0.41f);
            Gizmos.DrawSphere(transform.position, taskSize);
        }
    }
}