using Module.Player.Controller;
using Module.Player.State;
using VContainer;
using VContainer.Unity;
using Wanna.DebugEx;

namespace System.GameProgress
{
    /// <summary>
    /// ステージ進捗のプレゼンター
    /// </summary>
    public class GameSequenceHandler : IInitializable
    {
        private readonly StageProgressObserver progressObserver;
        private readonly PlayerController playerController;

        [Inject]
        public GameSequenceHandler(StageProgressObserver progressObserver, PlayerController playerController)
        {
            this.progressObserver = progressObserver;
            this.playerController = playerController;
        }

        public void Initialize()
        {
            progressObserver.OnCompleted += OnCompleted;
        }

        void OnCompleted()
        {
            playerController.SetState(PlayerState.Pause);
            DebugEx.Log("Completed!");
        }
    }
}