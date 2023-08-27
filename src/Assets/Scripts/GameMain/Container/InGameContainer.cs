using GameMain.Presenter;
using Module.Player.Controller;
using Module.Task;
using Module.Working;
using Module.Working.Controller;
using Module.Working.Factory;
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


        [SerializeField] private SpawnParam spawnParam = default!;
        [SerializeField] private GameParam gameParam = default!;
        [SerializeField] private WorkerController workerController = default!;
        [SerializeField] private TaskDetector taskDetector = default!;
        [SerializeField] private PlayerController playerController = default!;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<GameRouter>();
            builder.RegisterEntryPoint<TaskSystemLoop>();
            builder.RegisterEntryPoint<WorkerConnector>();

            builder.Register<WorkerSpawner>(Lifetime.Singleton);
            builder.Register<WorkerAgent>(Lifetime.Singleton);
            builder.Register<LeadPointConnector>(Lifetime.Singleton);

            builder.RegisterInstance(spawnPoint);
            builder.RegisterInstance(spawnParam);
            builder.RegisterInstance(gameParam);
            builder.RegisterInstance(taskDetector);
            builder.RegisterInstance(workerController);
            builder.RegisterInstance(playerController);
        }
    }
}