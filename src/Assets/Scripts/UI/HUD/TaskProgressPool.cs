using UnityEngine;
using UnityEngine.Pool;

namespace UI.HUD
{
    /// <summary>
    /// 進捗バーをプールするクラス
    /// </summary>
    public class TaskProgressPool : MonoBehaviour
    {
        [Header("進捗バーのプレハブ")]
        [SerializeField] private GameObject prefab;
        private ObjectPool<TaskProgressView> progressPool;

        private void Awake()
        {
            progressPool = new ObjectPool<TaskProgressView>(CreateView);
        }

        /// <summary>
        /// 進捗バーを取得します
        /// </summary>
        /// <param name="task">タスクのTransform</param>
        /// <returns>進捗バーのインスタンス</returns>
        public TaskProgressView GetProgressView(Transform task)
        {
            TaskProgressView progressView = progressPool.Get();
            progressView.SetTarget(task);
            progressView.Enable();

            return progressView;
        }

        /// <summary>
        /// 進捗バーを返却します
        /// </summary>
        /// <param name="view">進捗バーのインスタンス</param>
        public void ReleaseProgressView(TaskProgressView view)
        {
            view.SetTarget(null);
            view.Disable();

            progressPool.Release(view);
        }

        private TaskProgressView CreateView()
        {
            return Instantiate(prefab, transform).GetComponent<TaskProgressView>();
        }
    }
}