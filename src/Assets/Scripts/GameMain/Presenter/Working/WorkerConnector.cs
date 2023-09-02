using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Module.Task;
using VContainer;

namespace GameMain.Presenter.Working
{
    /// <summary>
    ///     ワーカーとタスクを仲介するクラス
    /// </summary>
    public class WorkerConnector 
    {
        private readonly List<Assignment> assignments;
        private readonly GameParam gameParam;
        private readonly TaskDetector taskDetector;

        private CancellationTokenSource loopCanceller;

        [Inject]
        public WorkerConnector(TaskDetector taskDetector, GameParam gameParam)
        {
            this.taskDetector = taskDetector;
            this.gameParam = gameParam;
            assignments = new List<Assignment>();
            loopCanceller = new CancellationTokenSource();

            CreateAssignments();
        }

        public IReadOnlyList<Assignment> Assignments => assignments;

        private void CreateAssignments()
        {
            foreach (var task in TaskUtil.FindSceneTasks<BaseTask>()) assignments.Add(new Assignment(task, task.transform));
        }

        public async UniTaskVoid StartLoop(Action<BaseTask> loopAction)
        {
            //既に行われているループを止める
            CancelLoop();

            var delay = TimeSpan.FromSeconds(gameParam.AssignInterval);

            while (!loopCanceller.IsCancellationRequested)
            {
                //タスクの検出
                taskDetector.UpdateDetection();

                var nearestTask = taskDetector.GetNearestTask();

                if (nearestTask != null) loopAction(nearestTask);

                await UniTask.Delay(delay, cancellationToken: loopCanceller.Token);
            }
        }

        public void CancelLoop()
        {
            loopCanceller.Cancel();
            loopCanceller.Dispose();

            loopCanceller = new CancellationTokenSource();
        }
    }
}