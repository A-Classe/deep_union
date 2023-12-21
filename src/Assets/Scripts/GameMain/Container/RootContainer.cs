using Core.Scenes;
using Core.User;
using Core.User.API;
using GameMain.Presenter;
using Module.GameManagement;
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
            builder.RegisterInstance(gameParam);
            builder.RegisterInstance(new SceneChanger());
            builder.RegisterInstance(new AudioMixerController(mixer));
            builder.Register<FirebaseAccessor>(Lifetime.Singleton);
            builder.Register<UserPreference>(Lifetime.Singleton);
            builder.Register<TimeManager>(Lifetime.Singleton);
        }
    }
}