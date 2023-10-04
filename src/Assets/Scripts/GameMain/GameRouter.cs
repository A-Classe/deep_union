using System;
using System.GameProgress;
using Core.Model.Scene;
using Core.NavMesh;
using Core.Scenes;
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
            SceneChanger sceneChanger
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
            playerController.InitParam(gameParam.ConvertToPlayerModel());
            playerController.PlayerStart();
            playerController.SetState(PlayerState.Go);

            cameraController.SetFollowTarget(playerController.transform);

            workerController.SetCamera(cameraController.GetCamera());
        }

        private void InitScene()
        {
            uiManager.SetSceneChanger(sceneChanger);
            uiManager.SetScreen(InGameNav.InGame);
            
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
        }

        public void Dispose()
        {
            progressObserver.Cancel();
        }

        public void Tick()
        {
            taskActivator.Tick();
        }
    }
}