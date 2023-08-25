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
        private readonly WorkerController workerController;

        public WorkerReleaser(WorkerConnector workerConnector, WorkerController workerController)
        {
            this.workerController = workerController;
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

                //作業量の更新
                assignment.Task.Mw -= 1;

                //コントローラーに登録
                workerController.EnqueueWorker(worker);
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
                foreach (var worker in assignment.Workers) workerController.EnqueueWorker(worker);

                //タスクからワーカーを削除
                assignment.Workers.Clear();

                //作業量の更新
                assignment.Task.Mw = 0f;
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
            var origin = workerController.transform.position;
            return assignment.Workers.OrderBy(worker => (origin - worker.transform.position).sqrMagnitude).First();
        }
    }
}