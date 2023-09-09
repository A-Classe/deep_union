using System.Collections.Generic;
using VContainer;

namespace Module.Working.Controller
{
    /// <summary>
    /// 追従するポイントをワーカーと紐付けるクラス
    /// </summary>
    public class LeadPointConnector
    {
        private readonly List<Worker> workers;
        public int WorkerCount => workers.Count;
        private bool isWorldMoving;

        [Inject]
        public LeadPointConnector(WorkerController target)
        {
            workers = new List<Worker>();
        }

        /// <summary>
        /// ワーカーの目標をリーダーに設定します
        /// </summary>
        /// <param name="worker">設定するワーカー</param>
        public void AddWorker(Worker worker)
        {
            workers.Add(worker);
            worker.IsWorldMoving = isWorldMoving;
        }

        public void RemoveWorker(Worker worker)
        {
            workers.Remove(worker);
            worker.IsWorldMoving = false;
        }

        public void SetWorldMovingActive(bool enable)
        {
            isWorldMoving = enable;

            foreach (Worker worker in workers)
            {
                worker.IsWorldMoving = enable;
            }
        }
    }
}