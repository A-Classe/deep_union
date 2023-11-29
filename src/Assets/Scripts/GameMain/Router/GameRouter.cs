using System;
using System.GameProgress;
using Core.Input;
using Core.Model.Scene;
using Core.NavMesh;
using Core.Scenes;
using Core.User;
using GameMain.Presenter;
using Module.Assignment.Component;
using Module.Player;
using Module.Player.Camera;
using Module.Player.Camera.State;
using Module.Player.Controller;
using Module.Player.State;
using Module.Task;
using Module.UI.InGame;
using Module.Working;
using Module.Working.Controller;
using Module.Working.Factory;
using VContainer;
using VContainer.Unity;

namespace GameMain.Router
{
    /// <summary>
    ///     ゲームのエントリーポイント
    /// </summary>
    public class GameRouter : IStartable, ITickable, IDisposable
    {
        private readonly LeaderAssignableArea leaderAssignableArea;

        private readonly PlayerController playerController;

        private readonly PlayerStatus playerStatus;

        private readonly UserPreference preference;

        private readonly StageProgressObserver progressObserver;

        private readonly ResourceContainer resourceContainer;
        private readonly RuntimeNavMeshBaker runtimeNavMeshBaker;

        private readonly SceneChanger sceneChanger;
        private readonly SpawnParam spawnParam;
        private readonly TaskActivator taskActivator;

        private readonly InGameUIManager uiManager;

        private readonly WorkerAgent workerAgent;
        private readonly WorkerController workerController;

        private readonly WorkerSpawner workerSpawner;

        [Inject]
        public GameRouter(
            SpawnParam spawnParam,
            GameParam gameParam,
            WorkerSpawner workerSpawner,
            WorkerController workerController,
            PlayerController playerController,
            StageProgressObserver progressObserver,
            RuntimeNavMeshBaker runtimeNavMeshBaker,
            TaskActivator taskActivator,
            LeaderAssignableArea leaderAssignableArea,
            InGameUIManager uiManager,
            SceneChanger sceneChanger,
            UserPreference preference,
            PlayerStatus playerStatus,
            ResourceContainer resourceContainer,
            WorkerAgent workerAgent
        )
        {
            this.spawnParam = spawnParam;

            this.playerController = playerController;
            this.workerController = workerController;

            this.progressObserver = progressObserver;
            this.runtimeNavMeshBaker = runtimeNavMeshBaker;
            this.taskActivator = taskActivator;
            this.leaderAssignableArea = leaderAssignableArea;

            this.workerSpawner = workerSpawner;

            this.uiManager = uiManager;

            this.sceneChanger = sceneChanger;

            this.preference = preference;

            this.playerStatus = playerStatus;

            this.resourceContainer = resourceContainer;

            this.workerAgent = workerAgent;
        }

        public void Dispose()
        {
            progressObserver.Cancel();
        }

        public void Start()
        {
            runtimeNavMeshBaker.Build();
            progressObserver.Start().Forget();

            InitWorker();

            InitPlayer();

            InitScene();
        }

        public void Tick()
        {
            taskActivator.Tick();

            // TODO: ステージの座標と距離感を決める
            var progress = (int)progressObserver.GetDistance();
            uiManager.UpdateStageProgress(progress);
        }

        /// <summary>
        ///     workerのセットアップ
        /// </summary>
        private void InitWorker()
        {
            var addedWorkers = workerSpawner.Spawn(spawnParam.SpawnCount);

            foreach (var worker in addedWorkers) leaderAssignableArea.AssignableArea.AddWorker(worker);
        }

        /// <summary>
        ///     PLayerのセットアップ
        /// </summary>
        private void InitPlayer()
        {
            playerController.PlayerStart();
            playerController.SetState(PlayerState.Auto);

            uiManager.OnGameInactive += OnCallGameInactive;
            uiManager.OnGameActive += OnCallGameActive;
        }

        private void InitScene()
        {
            uiManager.SetSceneChanger(sceneChanger);
            uiManager.StartGame(preference);

            progressObserver.OnCompleted += () =>
            {
                sceneChanger.LoadResult(
                    new GameResult
                    {
                        Hp = playerStatus.Hp,
                        Resource = resourceContainer.ResourceCount,
                        stageCode = (int)sceneChanger.GetInGame(),
                        WorkerCount = workerAgent.WorkerCount()
                    }
                );
            };

            var escEvent = InputActionProvider.Instance.CreateEvent(ActionGuid.InGame.ESC);
            escEvent.Canceled += _ =>
            {
                if (playerController.GetState() != PlayerState.Pause) uiManager.StartPause();
            };
        }

        // HPが0になった時 or オプション画面で
        private void OnCallGameInactive()
        {
            workerController.SetPlayed(false);
            playerController.SetState(PlayerState.Pause);
        }

        private void OnCallGameActive()
        {
            workerController.SetPlayed(true);
            playerController.SetState(PlayerState.Auto);
        }
    }
}