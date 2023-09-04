using System;
using System.Collections.Generic;
using System.Linq;
using Module.Task;
using Module.Working;
using Module.Working.Controller;
using Module.Working.State;
using UnityEngine;
using VContainer;
using Wanna.DebugEx;

namespace GameMain.Presenter.Working
{
    /// <summary>
    ///     ワーカーのアサイン処理を行うクラス
    /// </summary>
    public class WorkerAssigner
    {
        private readonly IReadOnlyList<Assignment> assignments;
        private readonly LeadPointConnector leadPointConnector;
        private readonly GameParam gameParam;

        public float LoopInterval => gameParam.AssignInterval;

        [Inject]
        public WorkerAssigner(WorkerConnector workerConnector, LeadPointConnector leadPointConnector, GameParam gameParam)
        {
            this.leadPointConnector = leadPointConnector;
            this.gameParam = gameParam;
            assignments = workerConnector.Assignments;
        }

        public void Assign(BaseTask nearestTask)
        {
            //タスクが完了していたらアサインしない
            if (nearestTask.State == TaskState.Completed)
                return;

            var position = nearestTask.transform.position;
            var nearestWorker = leadPointConnector.RemoveNearestWorker(position);

            if (nearestWorker == null)
                return;

            //アサインできる座標がなかったら終了
            if (!nearestTask.TryGetNearestAssignPoint(nearestWorker.transform.position, out Transform assignPoint))
            {
                leadPointConnector.AddWorker(nearestWorker);
                return;
            }

            try
            {
                UpdateTask(nearestTask, nearestWorker);
                UpdateWorker(nearestWorker, assignPoint);
            }
            catch (Exception e)
            {
                DebugEx.LogWarning("タスクが登録されていません!");
                DebugEx.LogException(e);
                throw;
            }
        }

        void UpdateTask(BaseTask task, Worker worker)
        {
            //ワーカーリストに登録
            var assignment = assignments.First(connect => connect.Task == task);
            assignment.Workers.Add(worker);

            //ワーカー数の更新
            assignment.Task.WorkerCount += 1;
        }

        void UpdateWorker(Worker nearestWorker, Transform assignPoint)
        {
            nearestWorker.SetFollowTarget(assignPoint, Vector3.zero);
            nearestWorker.SetWorkerState(WorkerState.Working);
        }
    }
}