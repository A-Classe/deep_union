using System;
using System.Collections.Generic;
using Core.Utility;
using UnityEngine;
using UnityEngine.Pool;
using VContainer;
using Wanna.DebugEx;
using Object = UnityEngine.Object;

namespace Module.Working
{
    /// <summary>
    ///     Workerのインスタンスを管理するクラス
    /// </summary>
    public class WorkerAgent
    {
        private const int workersCapacity = 128;
        private readonly List<Worker> activeWorkers;
        private readonly ObjectPool<Worker> workerPool;
        private readonly GameObject workerPrefab;

        [Inject]
        public WorkerAgent()
        {
            //プーリングする
            workerPool = new ObjectPool<Worker>(
                CreateWorker,
                OnWorkerGet,
                OnWorkerRelease,
                OnWorkerDestroy,
                defaultCapacity: workersCapacity);

            activeWorkers = new List<Worker>(workersCapacity);
            workerPrefab = Resources.Load<GameObject>("Worker");
        }

        /// <summary>
        ///     Workerを指定数追加します
        /// </summary>
        public Span<Worker> Add(int count)
        {
            for (var i = 0; i < count; i++)
            {
                var worker = workerPool.Get();
                activeWorkers.Add(worker);
            }

            return activeWorkers.AsSpan().Slice(activeWorkers.Count - count, count);
        }

        /// <summary>
        ///     Workerを指定数削除します
        /// </summary>
        public void Remove(Worker worker)
        {
            workerPool.Release(worker);
            activeWorkers.Remove(worker);
        }

        private Worker CreateWorker()
        {
            var obj = Object.Instantiate(workerPrefab);
            obj.name = $"Worker_{workerPool.CountAll}";

            if (obj.TryGetComponent(out Worker worker)) return worker;

            DebugEx.LogWarning($"WorkerAgent: {obj.name}に{nameof(Worker)}コンポーネントがアタッチされていません");
            return null;
        }

        private void OnWorkerGet(Worker worker)
        {
            worker.Enable();
        }

        private void OnWorkerRelease(Worker worker)
        {
            worker.Disable();
        }

        private void OnWorkerDestroy(Worker worker)
        {
            worker.Dispose();
        }
    }
}