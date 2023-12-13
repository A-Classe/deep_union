using System;
using System.GameProgress;
using Core.Input;
using Core.Model.Scene;
using Core.Model.User;
using Core.NavMesh;
using Core.Scenes;
using Core.User;
using Core.User.API;
using Core.User.Recorder;
using Debug;
using GameMain.Presenter;
using Module.Assignment.Component;
using Module.Minimap;
using Module.GameManagement;
using Module.GameSetting;
using Module.Player;
using Module.Player.Controller;
using Module.Player.State;
using Module.Task;
using Module.UI.InGame;
using Module.Working;
using Module.Working.Controller;
using Module.Working.Factory;
using UnityEngine;
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

        private readonly MiniMapBuilder miniMapBuilder;

        private readonly AudioMixerController audioMixerController;

        private readonly BrightController brightController;

        private readonly EventBroker eventBroker;

        private readonly GameActionRecorder recorder;
        private readonly FirebaseAccessor db;
        private readonly TimeManager timeManager;
        private readonly DebugGameClear debugGameClear;


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
            MiniMapBuilder miniMapBuilder,
            AudioMixerController audioMixerController,
            BrightController brightController,
            EventBroker eventBroker,
            GameActionRecorder recorder,
            FirebaseAccessor db,
            TimeManager timeManager,
            DebugGameClear debugGameClear
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

            this.miniMapBuilder = miniMapBuilder;

            this.audioMixerController = audioMixerController;

            this.brightController = brightController;

            this.eventBroker = eventBroker;

            this.recorder = recorder;

            this.db = db;

            this.timeManager = timeManager;

            this.debugGameClear = debugGameClear;
        }

        public void Dispose()
        {
            progressObserver.Cancel();
        }

        public void Start()
        {
            Application.targetFrameRate = 60;

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
            uiManager.OnGameInactive += OnCallGameInactive;
            uiManager.OnGameActive += OnCallGameActive;

            playerController.OnMoveDistance += distance =>
            {
                eventBroker.SendEvent(new MovePlayer(Math.Abs(distance)).Event());
            };

            workerController.OnMoveDistance += distance =>
            {
                eventBroker.SendEvent(new MoveWorkers(Math.Abs(distance)).Event());
            };
        }

        private void InitScene()
        {
            uiManager.SetSceneChanger(sceneChanger);
            uiManager.StartGame(preference, audioMixerController);

            // Game Clear
            progressObserver.OnCompleted += OnGameClear;
            debugGameClear.OnGameClearRequested += OnGameClear;

            var escEvent = InputActionProvider.Instance.CreateEvent(ActionGuid.UI.ESC);
            escEvent.Canceled += _ =>
            {
                uiManager.StartPause();
            };

            var minimapEvent = InputActionProvider.Instance.CreateEvent(ActionGuid.InGame.MiniMap);
            minimapEvent.Canceled += _ =>
            {
                if (playerController.GetState() != PlayerState.Pause)
                {
                    uiManager.StartMinimap();
                }
            };
            sceneChanger.OnBeforeChangeScene += SaveReport;

            preference.Load();

            preference.CompletedFirst();
            preference.Save();

            UserData data = preference.GetUserData();
            brightController.SetBrightness(data.bright.value / 10f);
            uiManager.SetBrightnessController(brightController);

            eventBroker.Clear();
            eventBroker.SendEvent(new GamePlay().Event());

            OnCallGameActive();

        }

        // HPが0になった時 or オプション画面で
        private void OnCallGameInactive()
        {
            timeManager.Pause();
            playerController.SetState(PlayerState.Pause);
        }

        private void OnCallGameActive()
        {
            timeManager.Resume();
            playerController.SetState(PlayerState.Auto);
        }

        private void OnGameClear()
        {
            eventBroker.SendEvent(new GameClear().Event());
            SaveReport();
            var result = new GameResult
            {
                Hp = playerStatus.Hp,
                Resource = resourceContainer.ResourceCount,
                stageCode = (int)sceneChanger.GetInGame(),
                WorkerCount = workerAgent.WorkerCount()
            };
            if (sceneChanger.GetInGame() == StageData.Stage.Tutorial)
            {
                sceneChanger.LoadResult(result);
            }
            else
            {
                sceneChanger.LoadAfterMovieInGame(result);
            }
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