using Module.Worker;
using Module.Worker.Controller;
using Module.Worker.Factory;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace System.Container
{
    public class InGameContainer : LifetimeScope
    {
        [Header("ここにシーン上のインスタンスを登録")]
        [SerializeField]
        private SpawnPoint spawnPoint;

        [SerializeField] private SpawnParam spawnParam;
        [SerializeField] private WorkerController workerController;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<GameRouter>();

            builder.Register<WorkerSpawner>(Lifetime.Singleton);
            builder.Register<WorkerAgent>(Lifetime.Singleton);

            builder.RegisterInstance(spawnPoint);
            builder.RegisterInstance(spawnParam);
            builder.RegisterInstance(workerController);
        }
    }
}