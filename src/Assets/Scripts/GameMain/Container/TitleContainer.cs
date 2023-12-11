using Core.User;
using Core.User.API;
using GameMain.Router;
using Module.Extension.System;
using Module.UI.Title.Credit;
using Module.UI.Title.Option;
using Module.UI.Title.Quit;
using Module.UI.Title.Ranking;
using Module.UI.Title.StageSelect;
using Module.UI.Title.Stats;
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
        [SerializeField] private StatsManager statsManager;
        [SerializeField] private RankingManager rankingManager;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<TitleRouter>();
            builder.RegisterEntryPoint<DebugControl>();

            builder.RegisterInstance(title);
            builder.RegisterInstance(quit);
            builder.RegisterInstance(option);
            builder.RegisterInstance(credit);
            builder.RegisterInstance(stageSelect);
            builder.RegisterInstance(statsManager);
            builder.RegisterInstance(rankingManager);
        }
    }
}