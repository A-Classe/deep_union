using System.GameProgress;
using System.Linq;
using Core.NavMesh;
using Core.User;
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
using Module.Player.Camera;
using Module.Player.Controller;
using Module.Task;
using Module.UI.HUD;
using Module.UI.InGame;
using Module.Working;
using Module.Working.Controller;
using Module.Working.Factory;
using UnityDebugSheet.Runtime.Core.Scripts;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
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
        [SerializeField] private PlayerStatusVisualizer playerStatusVisualizer;
        [SerializeField] private DebugSheet debugSheet;

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
            builder.RegisterEntryPoint<SceneDebugTool>();
            builder.RegisterEntryPoint<LeaderPresenter>();
            builder.RegisterEntryPoint<AssignmentSystem>();
            builder.RegisterEntryPoint<PlayerStatusUpdater>();

            builder.Register<WorkerSpawner>(Lifetime.Singleton);
            builder.Register<WorkerAgent>(Lifetime.Singleton);
            builder.Register<StageProgressObserver>(Lifetime.Singleton);
            builder.Register<RuntimeNavMeshBaker>(Lifetime.Singleton);
            builder.Register<ResourceContainer>(Lifetime.Singleton);
            builder.Register<WorkerAssigner>(Lifetime.Singleton);
            builder.Register<WorkerReleaser>(Lifetime.Singleton);
            builder.Register<TaskActivator>(Lifetime.Singleton);
            builder.Register<UserPreference>(Lifetime.Singleton);

            builder.RegisterInstance(spawnPoint);
            builder.RegisterInstance(spawnParam);
            builder.RegisterInstance(workerController);
            builder.RegisterInstance(playerController);
            builder.RegisterInstance(goalPoint);
            builder.RegisterInstance(progressPool);
            builder.RegisterInstance(leaderAssignableArea);
            builder.RegisterInstance(inGameUIManager);
            builder.RegisterInstance(new PlayerStatus(gameParam.ConvertToStatus()));
            builder.RegisterInstance(playerStatusVisualizer);
            builder.RegisterInstance(debugSheet);
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