using Core.User;
using UI.Title.Credit;
using UI.Title.Option;
using UI.Title.Quit;
using UI.Title.StageSelect;
using UI.Title.Title;
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