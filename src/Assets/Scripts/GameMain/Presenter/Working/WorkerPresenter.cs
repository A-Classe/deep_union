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
        [Inject]
        public WorkerPresenter() { }

        public void Initialize()
        {
            //入力の登録
            var assignEvent = InputActionProvider.Instance.CreateEvent(ActionGuid.InGame.Assign);


            var releaseEvent = InputActionProvider.Instance.CreateEvent(ActionGuid.InGame.Release);
        }
    }
}