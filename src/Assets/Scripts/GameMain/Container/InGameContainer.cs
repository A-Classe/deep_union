using System.GameProgress;
using System.Linq;
using Core.NavMesh;
using Core.User.Recorder;
using Core.Utility;
using Core.Utility.Player;
using Debug;
using GameMain.Presenter;
using GameMain.Presenter.Resource;
using GameMain.Presenter.Working;
using GameMain.Router;
using Module.Assignment.Component;
using Module.Assignment.System;
using Module.Extension.UI;
using Module.GameSetting;
using Module.Player;
using Module.Player.Controller;
using Module.Task;
using Module.UI.HUD;
using Module.UI.InGame;
using Module.Working;
using Module.Working.Controller;
using Module.Working.Factory;
using UnityEngine;
using UnityEngine.Serialization;
using VContainer;
using VContainer.Unity;

namespace GameMain.Container
{
    public class InGameContainer : LifetimeScope
    {
        [Header("ここにシーン上のインスタンスを登録")] [SerializeField]
        private SpawnPoint spawnPoint;

        [SerializeField] private GameParam gameParam;
        [SerializeField] private SpawnParam spawnParam;
        [SerializeField] private WorkerController workerController;
        [SerializeField] private PlayerController playerController;
        [SerializeField] private GoalPoint goalPoint;
        [SerializeField] private TaskProgressPool progressPool;
        [SerializeField] private WorkerSoundPlayer workerSoundPlayer;
        [SerializeField] private HealTaskPool healTaskPool;
        [SerializeField] private PlayerStatusVisualizer playerStatusVisualizer;

        [SerializeField] private InGameUIManager inGameUIManager;
        
        [SerializeField] private BrightController brightController;

        [FormerlySerializedAs("leaderAssignEvent")] [SerializeField]
        private LeaderAssignableArea leaderAssignableArea;


        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<GameRouter>();
            builder.RegisterEntryPoint<TaskSystemLoop>();
            builder.RegisterEntryPoint<GameSequenceHandler>();
            builder.RegisterEntryPoint<ProgressBarSwitcher>();
            builder.RegisterEntryPoint<ResourcePresenter>();
            builder.RegisterEntryPoint<WorkerPresenter>();
            builder.RegisterEntryPoint<LeaderPresenter>();
            builder.RegisterEntryPoint<AssignmentSystem>();
            builder.RegisterEntryPoint<PlayerStatusUpdater>();
            builder.RegisterEntryPoint<HealTaskPoolPresenter>();
            
            builder.Register<WorkerSpawner>(Lifetime.Singleton);
            builder.Register<WorkerAgent>(Lifetime.Singleton);
            builder.Register<StageProgressObserver>(Lifetime.Singleton);
            builder.Register<RuntimeNavMeshBaker>(Lifetime.Singleton);
            builder.Register<ResourceContainer>(Lifetime.Singleton);
            builder.Register<WorkerAssigner>(Lifetime.Singleton);
            builder.Register<WorkerReleaser>(Lifetime.Singleton);
            builder.Register<TaskActivator>(Lifetime.Singleton);
            builder.Register<EventBroker>(Lifetime.Singleton);
            builder.Register<GameActionRecorder>(Lifetime.Singleton);
            builder.Register<ActiveAreaCollector>(Lifetime.Singleton);
            builder.Register<DebugGameClear>(Lifetime.Singleton);

            builder.RegisterInstance(spawnPoint);
            builder.RegisterInstance(healTaskPool);
            builder.RegisterInstance(spawnParam);
            builder.RegisterInstance(workerController);
            builder.RegisterInstance(playerController);
            builder.RegisterInstance(goalPoint);
            builder.RegisterInstance(progressPool);
            builder.RegisterInstance(leaderAssignableArea);
            builder.RegisterInstance(inGameUIManager);
            builder.RegisterInstance(workerSoundPlayer);
            builder.RegisterInstance(new PlayerStatus(gameParam.ConvertToStatus()));
            builder.RegisterInstance(playerStatusVisualizer);
            builder.RegisterInstance(brightController);

            builder.RegisterBuildCallback(container =>
            {
                var injectables = FindObjectsByType<Component>(FindObjectsSortMode.None)
                    .OfType<IInjectable>()
                    .Select(injectable => (injectable as MonoBehaviour)?.gameObject);

                foreach (var injectable in injectables) container.InjectGameObject(injectable);
            });
        }
    }
}