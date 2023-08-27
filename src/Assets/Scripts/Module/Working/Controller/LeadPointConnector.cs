using System.Collections.Generic;
using System.Linq;
using Module.Working.Factory;
using Module.Working.State;
using UnityEngine;
using VContainer;

namespace Module.Working.Controller
{
    /// <summary>
    /// 追従するポイントをワーカーと紐付けるクラス
    /// </summary>
    public class LeadPointConnector
    {
        private readonly Transform target;
        private readonly List<Vector3> offsets;
        private readonly Dictionary<Worker, Vector3> workerPoints;

        [Inject]
        public LeadPointConnector(WorkerController target, SpawnPoint spawnPoint)
        {
            this.target = target.transform;
            workerPoints = new Dictionary<Worker, Vector3>();
            offsets = new List<Vector3>();

            foreach (Vector3 point in spawnPoint.GetSpawnPoints())
            {
                offsets.Add(point);
            }
        }

        /// <summary>
        /// ワーカーの目標をリーダーに設定します
        /// </summary>
        /// <param name="worker">設定するワーカー</param>
        public void AddWorker(Worker worker)
        {
            Vector3 offset = AddWorkerPoint(worker);

            worker.SetFollowTarget(target, offset);
            worker.SetWorkerState(WorkerState.Following);
        }

        /// <summary>
        /// 指定した座標に最も近いワーカーをリーダーから開放します
        /// </summary>
        /// <param name="position">目標座標</param>
        /// <returns>開放したワーカーのインスタンス</returns>
        public Worker RemoveNearestWorker(Vector3 position)
        {
            if (workerPoints.Count == 0)
                return null;

            var worker = workerPoints.OrderBy(worker => (position - worker.Key.transform.position).sqrMagnitude).First().Key;
            RemoveWorkerPoint(worker);

            return worker;
        }

        /// <summary>
        /// フォローターゲットの座標を返します
        /// </summary>
        /// <returns></returns>
        public Vector3 GetTargetPoint()
        {
            return target.position;
        }

        private Vector3 AddWorkerPoint(Worker worker)
        {
            offsets.Sort((a, b) =>
            {
                if (a.sqrMagnitude - b.sqrMagnitude > 0)
                {
                    return 1;
                }

                return -1;
            });

            Vector3 offset = offsets[0];
            workerPoints.Add(worker, offset);
            offsets.RemoveAt(0);

            return offset;
        }

        private void RemoveWorkerPoint(Worker worker)
        {
            Vector3 position = workerPoints[worker];
            workerPoints.Remove(worker);

            offsets.Add(position);
        }
    }
}