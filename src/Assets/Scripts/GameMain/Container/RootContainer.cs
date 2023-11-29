using Core.Scenes;
using Core.User;
using Core.User.API;
using Debug;
using GameMain.Presenter;
using Module.GameSetting;
using UnityEngine;
using UnityEngine.Audio;
using VContainer;
using VContainer.Unity;

namespace GameMain.Container
{
    public class RootContainer : LifetimeScope
    {
        [SerializeField] private GameParam gameParam;
        [SerializeField] private AudioMixer mixer;

        protected override void Configure(IContainerBuilder builder)
        {
            if (gameParam.EnableDebugger) builder.RegisterEntryPoint<RootDebugTool>();

            builder.RegisterInstance(gameParam);
            builder.RegisterInstance(new SceneChanger());
            builder.RegisterInstance(new AudioMixerController(mixer));
        }
    }
}