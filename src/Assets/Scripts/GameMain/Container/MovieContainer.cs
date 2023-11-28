using GameMain.Router;
using Module.Extension.UI;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace GameMain.Container
{
    public class MovieContainer: LifetimeScope
    {
        [SerializeField] private VideoPlayerController videoPlayer;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<MovieRouter>();
            builder.RegisterInstance(videoPlayer);
        }
    }
}