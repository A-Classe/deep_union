using System.Spawner;
using Controller;
using GameSystem;
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
            builder.RegisterEntryPoint<WorkerPresenter>();

            builder.Register<WorkerSpawner>(Lifetime.Singleton);
            builder.Register<WorkerFactory>(Lifetime.Singleton);

            builder.RegisterInstance(spawnPoint);
            builder.RegisterInstance(spawnParam);
            builder.RegisterInstance(workerController);
        }
    }
}