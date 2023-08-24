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
    /// ワーカーとタスクを仲介するクラス
    /// </summary>
    public class WorkerConnector : IInitializable
    {
        private readonly TaskDetector taskDetector;
        private readonly GameParam gameParam;
        private readonly List<Assignment> assignments;
        private readonly WorkerAssigner workerAssigner;
        private readonly WorkerReleaser workerReleaser;

        private CancellationTokenSource loopCanceller;

        public IReadOnlyList<Assignment> Assignments => assignments;

        [Inject]
        public WorkerConnector(WorkerController workerController, TaskDetector taskDetector, GameParam gameParam)
        {
            this.taskDetector = taskDetector;
            this.gameParam = gameParam;
            assignments = new List<Assignment>();
            loopCanceller = new CancellationTokenSource();

            CreateAssignments();

            workerAssigner = new WorkerAssigner(this, workerController);
            workerReleaser = new WorkerReleaser(this, workerController);
        }

        void CreateAssignments()
        {
            foreach (BaseTask task in TaskUtil.FindSceneTasks<BaseTask>())
            {
                assignments.Add(new Assignment(task, task.transform));
            }
        }

        public async UniTaskVoid StartLoop(Action<BaseTask> loopAction)
        {
            //既に行われているループを止める
            CancelLoop();

            TimeSpan delay = TimeSpan.FromSeconds(gameParam.AssignInterval);

            while (!loopCanceller.IsCancellationRequested)
            {
                //タスクの検出
                taskDetector.UpdateDetection();

                BaseTask nearestTask = taskDetector.GetNearestTask();

                if (nearestTask != null)
                {
                    loopAction(nearestTask);
                }

                await UniTask.Delay(delay, cancellationToken: loopCanceller.Token);
            }
        }

        public void CancelLoop()
        {
            loopCanceller.Cancel();
            loopCanceller.Dispose();

            loopCanceller = new CancellationTokenSource();
        }

        public void Initialize() { }
    }
}