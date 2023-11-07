using System;
using System.GameProgress;
using Core.Input;
using Core.Model.Scene;
using Core.NavMesh;
using Core.Scenes;
using Core.User;
using Core.Utility.Player;
using GameMain.Presenter;
using Module.Assignment.Component;
using Module.Player.Camera;
using Module.Player.Controller;
using Module.Player.State;
using Module.Task;
using Module.Working;
using Module.Working.Controller;
using Module.Working.Factory;
using UI.InGame;
using VContainer;
using VContainer.Unity;

namespace GameMain
{
    /// <summary>
    /// ゲームのエントリーポイント
    /// </summary>
    public class GameRouter : IStartable, ITickable, IDisposable
    {
        private readonly SpawnParam spawnParam;
        private readonly GameParam gameParam;

        private readonly PlayerController playerController;
        private readonly CameraController cameraController;
        private readonly WorkerController workerController;

        private readonly StageProgressObserver progressObserver;
        private readonly RuntimeNavMeshBaker runtimeNavMeshBaker;
        private readonly TaskActivator taskActivator;
        private readonly LeaderAssignableArea leaderAssignableArea;

        private readonly WorkerSpawner workerSpawner;

        private readonly InGameUIManager uiManager;

        private readonly SceneChanger sceneChanger;

        private readonly UserPreference preference;

        [Inject]
        public GameRouter(
            SpawnParam spawnParam,
            GameParam gameParam,
            WorkerSpawner workerSpawner,
            WorkerController workerController,
            PlayerController playerController,
            CameraController cameraController,
            StageProgressObserver progressObserver,
            RuntimeNavMeshBaker runtimeNavMeshBaker,
            TaskActivator taskActivator,
            LeaderAssignableArea leaderAssignableArea,
            InGameUIManager uiManager,
            SceneChanger sceneChanger,
            UserPreference preference
        )
        {
            this.spawnParam = spawnParam;
            this.gameParam = gameParam;

            this.playerController = playerController;
            this.cameraController = cameraController;
            this.workerController = workerController;

            this.progressObserver = progressObserver;
            this.runtimeNavMeshBaker = runtimeNavMeshBaker;
            this.taskActivator = taskActivator;
            this.leaderAssignableArea = leaderAssignableArea;

            this.workerSpawner = workerSpawner;

            this.uiManager = uiManager;

            this.sceneChanger = sceneChanger;

            this.preference = preference;
        }

        public void Start()
        {
            runtimeNavMeshBaker.Build();
            progressObserver.Start().Forget();

            InitWorker();

            InitPlayer();
            
            InitScene();
        }

        /// <summary>
        /// workerのセットアップ
        /// </summary>
        private void InitWorker()
        {
            ReadOnlySpan<Worker> addedWorkers = workerSpawner.Spawn(spawnParam.SpawnCount);

            foreach (Worker worker in addedWorkers)
            {
                leaderAssignableArea.AssignableArea.AddWorker(worker);
            }
        }

        /// <summary>
        /// PLayerのセットアップ
        /// </summary>
        private void InitPlayer()
        {
            playerController.InitParam(gameParam);
            playerController.PlayerStart();
            playerController.SetState(PlayerState.Go);

            cameraController.SetFollowTarget(playerController.transform);

            workerController.SetCamera(cameraController.GetCamera());

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
                        Hp = 20,
                        Resource = 30,
                        stageCode = (int) sceneChanger.GetInGame()
                    }
                );
            };
            
            var escEvent = InputActionProvider.Instance.CreateEvent(ActionGuid.InGame.ESC);
            escEvent.Canceled += _ =>
            {
                if (playerController.GetState() != PlayerState.Pause)
                {
                    uiManager.StartPause();
                }
            };
        }

        public void Dispose()
        {
            progressObserver.Cancel();
        }

        public void Tick()
        {
            taskActivator.Tick();
            
            // TODO: ステージの座標と距離感を決める
            int progress = (int)progressObserver.GetDistance();
            uiManager.UpdateStageProgress(progress);
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
            playerController.SetState(PlayerState.Go);
        }
    }
}