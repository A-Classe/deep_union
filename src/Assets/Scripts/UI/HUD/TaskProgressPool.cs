using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using Wanna.DebugEx;

namespace UI.HUD
{
    public class TaskProgressPool : MonoBehaviour
    {
        [SerializeField] private GameObject prefab;
        private ObjectPool<TaskProgressView> progressPool;

        private void Awake()
        {
            progressPool = new ObjectPool<TaskProgressView>(CreateView);
        }

        public TaskProgressView AddProgressView(Transform task)
        {
            TaskProgressView progressView = progressPool.Get();
            progressView.SetTarget(task);
            progressView.Enable();

            return progressView;
        }

        public void RemoveProgressView(TaskProgressView view)
        {
            TaskProgressView progressView = view;
            progressView.SetTarget(null);
            progressView.Disable();

            progressPool.Release(progressView);
        }

        private TaskProgressView CreateView()
        {
            return Instantiate(prefab, transform).GetComponent<TaskProgressView>();
        }
    }
}