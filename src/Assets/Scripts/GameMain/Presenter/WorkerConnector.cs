using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Module.Task;
using Module.Working.Controller;
using VContainer;
using VContainer.Unity;

namespace GameMain.Presenter
{
    /// <summary>
    ///     ワーカーとタスクを仲介するクラス
    /// </summary>
    public class WorkerConnector : IInitializable
    {
        private readonly List<Assignment> assignments;
        private readonly GameParam gameParam;
        private readonly TaskDetector taskDetector;

        // ReSharper disable once NotAccessedField.Local
        private readonly WorkerAssigner workerAssigner;

        // ReSharper disable once NotAccessedField.Local
        private readonly WorkerReleaser workerReleaser;

        private CancellationTokenSource loopCanceller;

        [Inject]
        public WorkerConnector(LeadPointConnector leadPointConnector, TaskDetector taskDetector, GameParam gameParam)
        {
            this.taskDetector = taskDetector;
            this.gameParam = gameParam;
            assignments = new List<Assignment>();
            loopCanceller = new CancellationTokenSource();

            CreateAssignments();

            workerAssigner = new WorkerAssigner(this, leadPointConnector);
            workerReleaser = new WorkerReleaser(this, leadPointConnector);
        }

        public IReadOnlyList<Assignment> Assignments => assignments;

        public void Initialize() { }

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