using Core.Utility.Player;
using GameMain.Presenter;
using Module.Player.Controller;
using Module.Player.State;
using Module.Working.Controller;
using Module.Working.Factory;
using VContainer;
using VContainer.Unity;

namespace GameMain
{
    public class GameRouter : IStartable
    {
        private readonly SpawnParam spawnParam;
        private readonly GameParam gameParam;

        private readonly LeadPointConnector leadPointConnector;
        private readonly PlayerController playerController;

        private readonly WorkerSpawner workerSpawner;

        [Inject]
        public GameRouter(
            SpawnParam spawnParam,
            GameParam gameParam,
            WorkerSpawner workerSpawner,
            LeadPointConnector leadPointConnector,
            PlayerController playerController
        )
        {
            this.spawnParam = spawnParam;
            this.gameParam = gameParam;

            this.leadPointConnector = leadPointConnector;
            this.playerController = playerController;

            this.workerSpawner = workerSpawner;
        }

        public void Start()
        {
            InitWorker();

            InitPlayer();
        }

        /// <summary>
        /// workerのセットアップ
        /// </summary>
        private void InitWorker()
        {
            var workers = workerSpawner.Spawn(spawnParam.SpawnCount);

            foreach (var worker in workers) leadPointConnector.AddWorker(worker);
        }

        /// <summary>
        /// PLayerのセットアップ
        /// </summary>
        private void InitPlayer()
        {
            playerController.InitParam(gameParam.ConvertToPlayerModel());
            playerController.PlayerStart();
            playerController.SetState(PlayerState.Go);
        }
    }
}