using System;
using System.GameProgress;
using Core.Input;
using Core.Model.Scene;
using Core.Model.User;
using Core.NavMesh;
using Core.Scenes;
using Core.User;
using Core.User.Recorder;
using GameMain.Presenter;
using Module.Assignment.Component;
using Module.GameSetting;
using Module.Player;
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
        private readonly GameParam gameParam;
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
        
        private readonly AudioMixerController audioMixerController;
        
        private readonly BrightController brightController;

        private readonly EventBroker eventBroker;

        private readonly GameActionRecorder recorder;

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
            WorkerAgent workerAgent,
            AudioMixerController audioMixerController,
            BrightController brightController,
            EventBroker eventBroker,
            GameActionRecorder recorder
        )
        {
            this.spawnParam = spawnParam;
            this.gameParam = gameParam;

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
            
            this.audioMixerController = audioMixerController;
            
            this.brightController = brightController;

            this.eventBroker = eventBroker;

            this.recorder = recorder;
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
            playerController.InitParam(gameParam);
            playerController.PlayerStart();
            playerController.SetState(PlayerState.Go);

            uiManager.OnGameInactive += OnCallGameInactive;
            uiManager.OnGameActive += OnCallGameActive;
        }

        private void InitScene()
        {
            uiManager.SetSceneChanger(sceneChanger);
            uiManager.StartGame(preference, audioMixerController);

            // Game Clear
            progressObserver.OnCompleted += OnGameClear;

            var escEvent = InputActionProvider.Instance.CreateEvent(ActionGuid.InGame.ESC);
            escEvent.Canceled += _ =>
            {
                if (playerController.GetState() != PlayerState.Pause) uiManager.StartPause();
            };

            sceneChanger.OnBeforeChangeScene += SaveReport;
            
            preference.Load();
            UserData data = preference.GetUserData();
            brightController.SetBrightness(data.bright.value / 10f);
            uiManager.SetBrightnessController(brightController);
            
            eventBroker.Clear();
            eventBroker.SendEvent(new GamePlay().Event());
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

        private void OnGameClear()
        {
            eventBroker.SendEvent(new GameClear().Event());
            sceneChanger.LoadResult(
                new GameResult
                {
                    Hp = playerStatus.Hp,
                    Resource = resourceContainer.ResourceCount,
                    stageCode = (int)sceneChanger.GetInGame(),
                    WorkerCount = workerAgent.WorkerCount()
                }
            );
        }

        private void SaveReport()
        {
            Report current = recorder.GenerateReport();
            Report data = preference.GetReport();
            preference.SetReport(data + current);
            preference.Save();
        }
    }
}