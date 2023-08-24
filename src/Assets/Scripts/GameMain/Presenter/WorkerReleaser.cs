using System;
using System.Collections.Generic;
using System.Linq;
using Core.Input;
using Module.Task;
using Module.Working;
using Module.Working.Controller;
using UnityEngine;
using Wanna.DebugEx;

namespace GameMain.Presenter
{
    
    /// <summary>
    /// ワーカーのリリース処理を行うクラス
    /// </summary>
    public class WorkerReleaser
    {
        private readonly WorkerController workerController;
        private readonly IReadOnlyList<Assignment> assignments;
        private readonly InputEvent releaseEvent;

        public WorkerReleaser(WorkerConnector workerConnector, WorkerController workerController)
        {
            this.workerController = workerController;
            assignments = workerConnector.Assignments;

            //入力の登録
            releaseEvent = InputActionProvider.Instance.CreateEvent(ActionGuid.InGame.Release);
            releaseEvent.Started += _ => workerConnector.StartLoop(Release).Forget();
            releaseEvent.Canceled += _ => workerConnector.CancelLoop();
        }

        void Release(BaseTask nearestTask)
        {
            IJobHandle jobHandle = nearestTask as IJobHandle;

            try
            {
                Assignment assignment = assignments.First(connect => connect.JobHandle == jobHandle);

                //タスクで働いてるワーカーが居なければ終了
                if (assignment.Workers.Count == 0)
                    return;

                //タスクからワーカーを削除
                Worker worker = GetNearestWorker(assignment);

                if (worker != null)
                {
                    assignment.Workers.Remove(worker);

                    //コントローラーに登録
                    workerController.EnqueueWorker(worker);
                }
            }
            catch (Exception e)
            {
                DebugEx.LogWarning("タスクが登録されていません!");
                DebugEx.LogException(e);
                throw;
            }
        }


        Worker GetNearestWorker(Assignment assignment)
        {
            Vector3 origin = workerController.transform.position;
            return assignment.Workers.OrderBy(worker => (origin - worker.transform.position).sqrMagnitude).First();
        }
    }
}