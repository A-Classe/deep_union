using System;
using System.Collections.Generic;
using System.Linq;
using Core.Input;
using Module.Task;
using Module.Working;
using Module.Working.Controller;
using Wanna.DebugEx;

namespace GameMain.Presenter
{
    /// <summary>
    ///     ワーカーのリリース処理を行うクラス
    /// </summary>
    public class WorkerReleaser
    {
        private readonly IReadOnlyList<Assignment> assignments;
        private readonly InputEvent releaseEvent;
        private readonly LeadPointConnector leadPointConnector;

        public WorkerReleaser(WorkerConnector workerConnector, LeadPointConnector leadPointConnector)
        {
            this.leadPointConnector = leadPointConnector;
            assignments = workerConnector.Assignments;

            //入力の登録
            releaseEvent = InputActionProvider.Instance.CreateEvent(ActionGuid.InGame.Release);
            releaseEvent.Started += _ => workerConnector.StartLoop(Release).Forget();
            releaseEvent.Canceled += _ => workerConnector.CancelLoop();

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

        private void Release(BaseTask nearestTask)
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


        void UpdateTask(BaseTask task, Worker worker) { }


        private Worker GetNearestWorker(Assignment assignment)
        {
            var origin = leadPointConnector.GetTargetPoint();
            return assignment.Workers.OrderBy(worker => (origin - worker.transform.position).sqrMagnitude).First();
        }
    }
}