using Debug;
using GameMain.Presenter;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace GameMain.Container
{
    public class RootContainer : LifetimeScope
    {
        [SerializeField] private GameParam gameParam;
        
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<RootDebugTool>();
            
            builder.RegisterInstance(gameParam);
        }
    }
}