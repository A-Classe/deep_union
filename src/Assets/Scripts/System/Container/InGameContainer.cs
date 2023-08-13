using GameSystem;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace System.Container
{
    public class InGameContainer : LifetimeScope
    {
        [SerializeField] private SpawnPoint spawnPoint;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<WorkerPresenter>();
            
            builder.Register<WorkerSpawner>(Lifetime.Singleton);
            builder.Register<WorkerFactory>(Lifetime.Singleton);

            builder.RegisterInstance(spawnPoint);
        }
    }
}