using System.GameProgress;
using Core.NavMesh;
using Debug;
using GameMain.Presenter;
using GameMain.Presenter.Resource;
using GameMain.Presenter.Working;
using GameMain.System;
using GameMain.UI;
using Module.Assignment;
using Module.Player.Camera;
using Module.Player.Controller;
using Module.Task;
using Module.Working;
using Module.Working.Controller;
using Module.Working.Factory;
using UI.HUD;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace GameMain.Container
{
    public class InGameContainer : LifetimeScope
    {
        [Header("ここにシーン上のインスタンスを登録")]
        [SerializeField]
        private SpawnPoint spawnPoint;

        [SerializeField] private SpawnParam spawnParam;
        [SerializeField] private WorkerController workerController;
        [SerializeField] private PlayerController playerController;
        [SerializeField] private CameraController cameraController;
        [SerializeField] private GoalPoint goalPoint;
        [SerializeField] private TaskProgressPool progressPool;

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
            builder.RegisterEntryPoint<LightDetectionConnector>();

            builder.Register<WorkerSpawner>(Lifetime.Singleton);
            builder.Register<WorkerAgent>(Lifetime.Singleton);
            builder.Register<LeadPointConnector>(Lifetime.Singleton);
            builder.Register<StageProgressObserver>(Lifetime.Singleton);
            builder.Register<RuntimeNavMeshBaker>(Lifetime.Singleton);
            builder.Register<ResourceContainer>(Lifetime.Singleton);
            builder.Register<LightDetector>(Lifetime.Singleton);
            builder.Register<TaskActivator>(Lifetime.Singleton);

            builder.RegisterInstance(spawnPoint);
            builder.RegisterInstance(spawnParam);
            builder.RegisterInstance(workerController);
            builder.RegisterInstance(playerController);
            builder.RegisterInstance(cameraController);
            builder.RegisterInstance(goalPoint);
            builder.RegisterInstance(progressPool);
        }
    }
}