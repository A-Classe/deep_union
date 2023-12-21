using Core.Input;
using Core.User.Recorder;
using Module.Assignment.Component;
using Module.Task;
using Module.Working;
using VContainer;
using VContainer.Unity;

namespace Module.Assignment.System
{
    /// <summary>
    ///     アサイン機能のループ処理を管理するクラス
    /// </summary>
    public class AssignmentSystem : ITickable, IStartable
    {
        private readonly InputEvent assignEvent;
        private readonly InputEvent releaseEvent;
        private readonly TaskActivator taskActivator;
        private readonly WorkerAssigner workerAssigner;
        private readonly WorkerReleaser workerReleaser;
        private readonly EventBroker eventBroker;
        private AssignState assignState = AssignState.Idle;

        [Inject]
        public AssignmentSystem(
            WorkerAssigner workerAssigner,
            WorkerReleaser workerReleaser,
            TaskActivator taskActivator,
            WorkerSoundPlayer workerAudioPlayer,
            ActiveAreaCollector activeAreaCollector,
            EventBroker eventBroker
        )
        {
            this.workerAssigner = workerAssigner;
            this.workerReleaser = workerReleaser;
            this.taskActivator = taskActivator;

            //入力イベントの登録
            assignEvent = InputActionProvider.Instance.CreateEvent(ActionGuid.InGame.Assign);
            releaseEvent = InputActionProvider.Instance.CreateEvent(ActionGuid.InGame.Release);

            assignEvent.Started += _ =>
            {
                assignState = AssignState.Assign;
            };

            assignEvent.Canceled += _ =>
            {
                assignState = AssignState.Idle;
            };

            releaseEvent.Started += _ =>
            {
                assignState = AssignState.Release;
            };

            releaseEvent.Canceled += _ =>
            {
                assignState = AssignState.Idle;
            };

            workerAssigner.OnAssign += workerAudioPlayer.PlayOnAssign;
            workerReleaser.OnRelease += workerAudioPlayer.PlayOnRelease;
        }

        public void Start()
        {
            //タスク完了時のコールバックを登録
            taskActivator.OnTaskInitialized += _ =>
            {
                foreach (var task in taskActivator.GetAllTasks())
                {
                    task.OnCompleted += OnTaskCompleted;
                }
            };

            taskActivator.OnTaskDeactivated += OnTaskCompleted;

            taskActivator.Start();
        }

        public void Tick()
        {
            switch (assignState)
            {
                case AssignState.Assign:
                    workerAssigner.Update();
                    break;
                case AssignState.Release:
                    workerReleaser.Update();
                    break;
            }
        }

        private void OnTaskCompleted(BaseTask task)
        {
            //完了したら自動的に子機に戻す
            workerReleaser.ReleaseAllWorkers(task.GetComponentInChildren<AssignableArea>());
        }
    }
}