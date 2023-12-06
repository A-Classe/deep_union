using System.Collections.Concurrent;
using UnityEngine;

namespace Core.Utility
{
    public class MainThreadDispatcher : MonoBehaviour
    {
        private static MainThreadDispatcher _instance;
        private readonly ConcurrentQueue<System.Action> _actions = new ConcurrentQueue<System.Action>();

        public static MainThreadDispatcher Instance
        {
            get
            {
                if (_instance == null)
                {
                    Debug.LogError("MainThreadDispatcher instance not found. Ensure it is initialized in the scene.");
                }
                return _instance;
            }
        }

        private void Awake()
        {
            if (_instance != null)
            {
                Debug.LogError("Another instance of MainThreadDispatcher already exists.");
                Destroy(this);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }


        public void Update()
        {
            while (_actions.TryDequeue(out var action))
            {
                action.Invoke();
            }
        }

        public void Enqueue(System.Action action)
        {
            _actions.Enqueue(action);
        }
    }
}