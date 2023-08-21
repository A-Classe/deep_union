using System;
using UnityEngine;

namespace Module.Task
{
    /// <summary>
    /// 全てのタスクのベースクラス
    /// </summary>
    [RequireComponent(typeof(SphereCollider))]
    [DisallowMultipleComponent]
    public abstract class BaseTask : MonoBehaviour, ITaskSystem, IJobHandle
    {
        [Header("検出されるタスクの半径")]
        [SerializeField]
        [Range(0f, 6f)]
        private float taskSize = 1f;

        public TaskState State => state;
        [SerializeField] protected TaskState state = TaskState.Idle;

        private void OnValidate()
        {
            SphereCollider col = GetComponent<SphereCollider>();
            col.radius = taskSize;
        }

        /// <summary>
        /// ゲーム開始時の初期化関数
        /// </summary>
        public virtual void Initialize() { }

        /// <summary>
        /// ゲームの状態によって管理されるUpdate
        /// </summary>
        /// <param name="deltaTime">Time.deltaTime</param>
        public virtual void ManagedUpdate(float deltaTime) { }

        /// <summary>
        /// ワーカーからJobを受信された際に実行される
        /// </summary>
        public abstract void ExecuteJob();

        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(0f, 0.83f, 0f, 0.41f);
            Gizmos.DrawSphere(transform.position, taskSize);
        }
    }
}