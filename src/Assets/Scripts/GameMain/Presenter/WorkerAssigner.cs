using System;
using System.Collections.Generic;
using System.Linq;
using Core.Input;
using Module.Task;
using Module.Working.Controller;
using Module.Working.State;
using Wanna.DebugEx;

namespace GameMain.Presenter
{
    /// <summary>
    ///     ワーカーのアサイン処理を行うクラス
    /// </summary>
    public class WorkerAssigner
    {
        private readonly InputEvent assignEvent;
        private readonly IReadOnlyList<Assignment> assignments;
        private readonly WorkerController workerController;

        public WorkerAssigner(WorkerConnector workerConnector, WorkerController workerController)
        {
            this.workerController = workerController;
            assignments = workerConnector.Assignments;

            //入力の登録
            assignEvent = InputActionProvider.Instance.CreateEvent(ActionGuid.InGame.Assign);
            assignEvent.Started += _ => workerConnector.StartLoop(Assign).Forget();
            assignEvent.Canceled += _ => workerConnector.CancelLoop();
        }

        private void Assign(BaseTask nearestTask)
        {
            //タスクが完了していたらアサインしない
            if (nearestTask.State == TaskState.Completed)
                return;

            var position = nearestTask.transform.position;
            var nearestWorker = workerController.DequeueNearestWorker(position);

            if (nearestWorker == null)
                return;

            try
            {
                //ワーカーリストに登録
                var assignment = assignments.First(connect => connect.Task == nearestTask);
                assignment.Workers.Add(nearestWorker);

                //作業量の更新
                assignment.Task.Mw += 1;

                nearestWorker.SetFollowTarget(assignment.Target);
                nearestWorker.SetWorkerState(WorkerState.Working);
            }
            catch (Exception e)
            {
                DebugEx.LogWarning("タスクが登録されていません!");
                DebugEx.LogException(e);
                throw;
            }
        }
    }
}