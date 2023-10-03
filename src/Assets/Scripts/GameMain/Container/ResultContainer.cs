using Core.User;
using UI.Result;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace GameMain.Container
{
    public class ResultContainer: LifetimeScope
    {
        [SerializeField] private ResultManager resultManager;
        
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<ResultRouter>();
            
            builder.Register<UserPreference>(Lifetime.Singleton);

            builder.RegisterInstance(resultManager);

        }
    }
}