using System;
using System.Collections.Generic;
using System.Linq;
using Module.Task;
using Module.Working;
using Module.Working.Controller;
using VContainer;
using Wanna.DebugEx;

namespace GameMain.Presenter.Working
{
    /// <summary>
    ///     ワーカーのリリース処理を行うクラス
    /// </summary>
    public class WorkerReleaser
    {
        private readonly IReadOnlyList<Assignment> assignments;
        private readonly LeadPointConnector leadPointConnector;
        private readonly GameParam gameParam;

        public float LoopInterval => gameParam.ReleaseInteval;

        [Inject]
        public WorkerReleaser(WorkerConnector workerConnector, LeadPointConnector leadPointConnector, GameParam gameParam)
        {
            this.leadPointConnector = leadPointConnector;
            this.gameParam = gameParam;
            assignments = workerConnector.Assignments;

            RegisterReleaseEvents();
        }

        private void RegisterReleaseEvents()
        {
            foreach (var assignment in assignments)
                assignment.Task.OnStateChanged += state =>
                {
                    //タスクが完了したら全てを開放する
                    if (state == TaskState.Completed) ReleaseAll(assignment.Task);
                };
        }

        public void Release(BaseTask nearestTask)
        {
            try
            {
                var assignment = assignments.First(connect => connect.Task == nearestTask);

                //タスクで働いてるワーカーが居なければ終了
                if (assignment.Workers.Count == 0)
                    return;

                //タスクからワーカーを削除
                var worker = GetNearestWorker(assignment);

                if (worker == null) return;
                assignment.Workers.Remove(worker);

                //ワーカー数の更新
                assignment.Task.WorkerCount -= 1;
                nearestTask.ReleaseAssignPoint(worker.Target);

                //コントローラーに登録
                leadPointConnector.AddWorker(worker);
            }
            catch (Exception e)
            {
                DebugEx.LogWarning("タスクが登録されていません!");
                DebugEx.LogException(e);
                throw;
            }
        }

        private void ReleaseAll(BaseTask baseTask)
        {
            try
            {
                var assignment = assignments.First(connect => connect.Task == baseTask);

                //コントローラーに登録
                foreach (var worker in assignment.Workers)
                {
                    leadPointConnector.AddWorker(worker);
                    baseTask.ReleaseAssignPoint(worker.Target);
                }

                //タスクからワーカーを削除
                assignment.Workers.Clear();

                //ワーカー数の初期化
                assignment.Task.WorkerCount = 0;
            }
            catch (Exception e)
            {
                DebugEx.LogWarning("タスクが登録されていません!");
                DebugEx.LogException(e);
                throw;
            }
        }

        private Worker GetNearestWorker(Assignment assignment)
        {
            var origin = leadPointConnector.GetTargetPoint();
            return assignment.Workers.OrderBy(worker => (origin - worker.transform.position).sqrMagnitude).First();
        }
    }
}