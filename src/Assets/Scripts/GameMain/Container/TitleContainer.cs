using Core.User;
using GameMain.Router;
using Module.UI.Title.Credit;
using Module.UI.Title.Option;
using Module.UI.Title.Quit;
using Module.UI.Title.StageSelect;
using Module.UI.Title.Title;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace GameMain.Container
{
    public class TitleContainer : LifetimeScope
    {
        [SerializeField] private TitleManager title;
        [SerializeField] private QuitManager quit;
        [SerializeField] private OptionManager option;
        [SerializeField] private CreditManager credit;
        [SerializeField] private StageSelectManager stageSelect;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<TitleRouter>();

            builder.Register<UserPreference>(Lifetime.Singleton);

            builder.RegisterInstance(title);
            builder.RegisterInstance(quit);
            builder.RegisterInstance(option);
            builder.RegisterInstance(credit);
            builder.RegisterInstance(stageSelect);
        }
    }
}