using Core.Utility.Player;
using Debug;
using GameMain.Presenter;
using Module.Assignment;
using UnityDebugSheet.Runtime.Core.Scripts;
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
            if (gameParam.EnableDebugger)
            {
                builder.RegisterEntryPoint<RootDebugTool>();
            }

            builder.RegisterInstance(gameParam);

            status = new PlayerStatus(gameParam.ConvertToStatus());

            builder.RegisterInstance(status);
        }
    }
}