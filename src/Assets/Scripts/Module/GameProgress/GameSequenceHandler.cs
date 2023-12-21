using Module.Player.Controller;
using Module.Player.State;
using VContainer;
using VContainer.Unity;
using Wanna.DebugEx;

namespace System.GameProgress
{
    /// <summary>
    ///     ステージ進捗のプレゼンター
    /// </summary>
    public class GameSequenceHandler : IInitializable, IDisposable
    {
        private readonly PlayerController playerController;
        private readonly StageProgressObserver progressObserver;

        [Inject]
        public GameSequenceHandler(StageProgressObserver progressObserver, PlayerController playerController)
        {
            this.progressObserver = progressObserver;
            this.playerController = playerController;
        }

        public void Dispose() { }

        public void Initialize()
        {
            progressObserver.OnCompleted += OnCompleted;
        }

        private void OnCompleted()
        {
            playerController.SetState(PlayerState.Pause);
            DebugEx.Log("Completed!");
        }
    }
}