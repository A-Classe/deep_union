using Core.Input;
using VContainer;
using VContainer.Unity;

namespace GameMain.Presenter.Working
{
    /// <summary>
    /// ワーカーアサイン機能のプレゼンタークラス
    /// </summary>
    public class WorkerPresenter : IInitializable
    {
        private readonly WorkerConnector workerConnector;
        private readonly WorkerAssigner workerAssigner;
        private readonly WorkerReleaser workerReleaser;

        [Inject]
        public WorkerPresenter(WorkerConnector workerConnector, WorkerAssigner workerAssigner, WorkerReleaser workerReleaser)
        {
            this.workerConnector = workerConnector;
            this.workerAssigner = workerAssigner;
            this.workerReleaser = workerReleaser;
        }

        public void Initialize()
        {
            //入力の登録
            var assignEvent = InputActionProvider.Instance.CreateEvent(ActionGuid.InGame.Assign);
            assignEvent.Started += _ => workerConnector.StartLoop(workerAssigner.Assign).Forget();
            assignEvent.Canceled += _ => workerConnector.CancelLoop();

            var releaseEvent = InputActionProvider.Instance.CreateEvent(ActionGuid.InGame.Release);
            releaseEvent.Started += _ => workerConnector.StartLoop(workerReleaser.Release).Forget();
            releaseEvent.Canceled += _ => workerConnector.CancelLoop();
        }
    }
}