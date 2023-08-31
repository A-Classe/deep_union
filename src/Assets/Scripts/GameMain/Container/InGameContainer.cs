using System.GameProgress;
using Core.NavMesh;
using GameMain.Presenter;
using GameMain.UI;
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
        [SerializeField] private GameParam gameParam;
        [SerializeField] private WorkerController workerController;
        [SerializeField] private TaskDetector taskDetector;
        [SerializeField] private PlayerController playerController;
        [SerializeField] private CameraController cameraController;
        [SerializeField] private GoalPoint goalPoint;
        [SerializeField] private TaskProgressPool progressPool;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<GameRouter>();
            builder.RegisterEntryPoint<TaskSystemLoop>();
            builder.RegisterEntryPoint<WorkerConnector>();
            builder.RegisterEntryPoint<GameSequenceHandler>();
            builder.RegisterEntryPoint<ProgressBarSwitcher>();

            builder.Register<WorkerSpawner>(Lifetime.Singleton);
            builder.Register<WorkerAgent>(Lifetime.Singleton);
            builder.Register<LeadPointConnector>(Lifetime.Singleton);
            builder.Register<StageProgressObserver>(Lifetime.Singleton);
            builder.Register<RuntimeNavMeshBaker>(Lifetime.Singleton);

            builder.RegisterInstance(spawnPoint);
            builder.RegisterInstance(spawnParam);
            builder.RegisterInstance(gameParam);
            builder.RegisterInstance(taskDetector);
            builder.RegisterInstance(workerController);
            builder.RegisterInstance(playerController);
            builder.RegisterInstance(cameraController);
            builder.RegisterInstance(goalPoint);
            builder.RegisterInstance(progressPool);
        }
    }
}