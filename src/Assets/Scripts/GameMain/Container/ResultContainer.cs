using Core.User;
using GameMain.Router;
using Module.Extension.UI;
using Module.UI.Result;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace GameMain.Container
{
    public class ResultContainer : LifetimeScope
    {
        [SerializeField] private ResultManager resultManager;
        [SerializeField] private VideoPlayerControllerExt videoPlayer;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<ResultRouter>();

            builder.Register<UserPreference>(Lifetime.Singleton);

            builder.RegisterInstance(resultManager);
            builder.RegisterInstance(videoPlayer);
        }
    }
}