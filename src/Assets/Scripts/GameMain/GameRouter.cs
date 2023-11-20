using System;
using System.GameProgress;
using Core.Input;
using Core.Model.Scene;
using Core.NavMesh;
using Core.Scenes;
using Core.User;
using GameMain.Presenter;
using Module.Assignment.Component;
using Module.Minimap;
using Module.Player;
using Module.Player.Camera;
using Module.Player.Camera.State;
using Module.Player.Controller;
using Module.Player.State;
using Module.Task;
using Module.Working;
using Module.Working.Controller;
using Module.Working.Factory;
using UI.InGame;
using VContainer;
using VContainer.Unity;
using Wanna.DebugEx;

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

        private readonly PlayerStatus playerStatus;

        private readonly ResourceContainer resourceContainer;

        private readonly WorkerAgent workerAgent;

        private readonly MiniMapBuilder miniMapBuilder;

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
            UserPreference preference,
            PlayerStatus playerStatus,
            ResourceContainer resourceContainer,
            WorkerAgent workerAgent,
            MiniMapBuilder miniMapBuilder
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

            this.playerStatus = playerStatus;

            this.resourceContainer = resourceContainer;
            
            this.workerAgent = workerAgent;

            this.miniMapBuilder = miniMapBuilder;
        }

        public void Start()
        {
            runtimeNavMeshBaker.Build();
            progressObserver.Start().Forget();
            
            InitMinimap();

            InitWorker();

            InitPlayer();
            
            InitScene();
        }

        private void InitMinimap()
        {
            miniMapBuilder.OnBuildFinished += buildData =>
            {
                uiManager.SetMinimapParam(buildData);
            };
            miniMapBuilder.Build();
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
            cameraController.SetState(CameraState.Follow);

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
                        Hp = playerStatus.Hp,
                        Resource = resourceContainer.ResourceCount,
                        stageCode = (int) sceneChanger.GetInGame(),
                        WorkerCount = workerAgent.WorkerCount()
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

            var minimapEvent = InputActionProvider.Instance.CreateEvent(ActionGuid.InGame.MiniMap);
            minimapEvent.Canceled += _ =>
            {
                if (playerController.GetState() != PlayerState.Pause)
                {
                    uiManager.StartMinimap();
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
            cameraController.SetState(CameraState.Idle);
        }

        private void OnCallGameActive()
        {
            workerController.SetPlayed(true);
            playerController.SetState(PlayerState.Go);
            cameraController.SetState(CameraState.Follow);
        }
    }
}