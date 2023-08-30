using Core.Utility.User;
using UI.Title.Credit;
using UI.Title.Option1;
using UI.Title.Option2;
using UI.Title.Option3;
using UI.Title.Option4;
using UI.Title.Quit;
using UI.Title.StageSelect;
using UI.Title.Title;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace UI.Title
{
    public class TitleContainer : LifetimeScope
    {
        [SerializeField] private TitleManager title;
        [SerializeField] private QuitManager quit;
        [SerializeField] private Option1Manager option1;
        [SerializeField] private Option2Manager option2;
        [SerializeField] private Option3Manager option3;
        [SerializeField] private Option4Manager option4;
        [SerializeField] private CreditManager credit;
        [SerializeField] private StageSelectManager stageSelect;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<TitleRouter>();

            builder.Register<UserPreference>(Lifetime.Singleton);

            builder.RegisterInstance(title);
            builder.RegisterInstance(quit);
            builder.RegisterInstance(option1);
            builder.RegisterInstance(option2);
            builder.RegisterInstance(option3);
            builder.RegisterInstance(option4);
            builder.RegisterInstance(credit);
            builder.RegisterInstance(stageSelect);
        }
    }
}