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

        public ReadOnlySpan<Worker> ActiveWorkers => activeWorkers.AsSpan();

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
        public ReadOnlySpan<Worker> Add(int count)
        {
            for (var i = 0; i < count; i++)
            {
                var worker = workerPool.Get();
                activeWorkers.Add(worker);
                worker.OnDead += Remove;
            }

            return activeWorkers.AsSpan().Slice(activeWorkers.Count - count, count);
        }

        /// <summary>
        ///    外部からのWorkerを追加します
        /// </summary>
        public void AddActiveWorker(Worker worker)
        {
            activeWorkers.Add(worker);
            worker.OnDead += Remove;
        }

        /// <summary>
        ///     Workerを指定数削除します
        /// </summary>
        // ReSharper disable once UnusedMember.Global
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

        private static void OnWorkerGet(Worker worker)
        {
            worker.Enable();
        }

        private static void OnWorkerRelease(Worker worker)
        {
            worker.Disable().Forget();
        }

        private static void OnWorkerDestroy(Worker worker)
        {
            worker.Dispose();
        }
    }
}