using Core.Utility.Player;
using Debug;
using GameMain.Presenter;
using GameMain.System;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace GameMain.Container
{
    public class RootContainer : LifetimeScope
    {
        [SerializeField] private GameParam gameParam;
        
        private PlayerStatus status;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<RootDebugTool>();
            
            builder.RegisterInstance(gameParam);
            
            status = new PlayerStatus(gameParam.ConvertToStatus());

            builder.RegisterInstance(status);
        }
    }
}