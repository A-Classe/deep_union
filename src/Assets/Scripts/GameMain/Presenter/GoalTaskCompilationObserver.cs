using System;
using Module.Task;
using UnityEngine;

namespace GameMain.Presenter
{
    public class GoalTaskCompilationObserver : MonoBehaviour
    {
        private BaseTask baseTask;
        public event Action OnCompleted;
        
        private void Start()
        {
            baseTask = GetComponent<BaseTask>();
            baseTask.OnCompleted += OnTaskCompleted;
        }

        private void OnTaskCompleted(BaseTask task)
        {
            OnCompleted?.Invoke();
        }
    }
}