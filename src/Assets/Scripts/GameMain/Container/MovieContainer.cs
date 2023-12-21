using Core.Utility.UI.Component;
using GameMain.Router;
using Module.Extension.UI;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace GameMain.Container
{
    public class MovieContainer : LifetimeScope
    {
        [SerializeField] private VideoPlayerController videoPlayer;
        [SerializeField] private HoldVisibleObject holdVisibleObject;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<MovieRouter>();
            builder.RegisterInstance(videoPlayer);
            builder.RegisterInstance(holdVisibleObject);
        }
    }
}