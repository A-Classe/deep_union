using System;
using System.Collections.Generic;
using Core.Scenes;
using Core.Utility.UI.Navigation;
using Core.Utility.User;
using GameMain.System.Scenes;
using UI.Title.Credit;
using UI.Title.Option;
using UI.Title.Quit;
using UI.Title.StageSelect;
using UI.Title.Title;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace UI.Title
{
    internal class TitleRouter : IStartable
    {
        private readonly CreditManager credit;

        private readonly UserPreference data;

        private readonly Navigation<TitleNavigation> navigation;


        private readonly OptionManager option;
        private readonly QuitManager quit;
        private readonly StageSelectManager stageSelect;
        private readonly TitleManager title;
        private readonly SceneChanger sceneChanger;

        [Inject]
        public TitleRouter(
            TitleManager titleManager,
            QuitManager quitManager,
            OptionManager optionManager,
            CreditManager creditManager,
            StageSelectManager stageSelectManager,
            UserPreference dataManager,
            SceneChanger sceneChanger
        )
        {
            title = titleManager;
            quit = quitManager;
            option = optionManager;
            credit = creditManager;
            stageSelect = stageSelectManager;
            this.sceneChanger = sceneChanger;

            data = dataManager;
            option.SetPreference(data);
            var initialManagers = new Dictionary<TitleNavigation, UIManager>
            {
                { TitleNavigation.Title, title },
                { TitleNavigation.Quit, quit },
                { TitleNavigation.Option, option },
                { TitleNavigation.Credit, credit },
                { TitleNavigation.StageSelect, stageSelect }
            };
            navigation = new Navigation<TitleNavigation>(initialManagers);
        }


        public void Start()
        {
            SetNavigation();
            NavigateToTitle();


            data.Delete();
            data.Load();
        }


        private void SetNavigation()
        {
            navigation.OnCancel += _ => { OnCanceled(); };

            title.OnQuit += NavigateToQuit;
            title.OnOption += NavigateToOption;
            title.OnCredit += NavigateToCredit;
            title.OnPlay += NavigateToPlay;

            quit.OnClick += isQuit =>
            {
                if (isQuit)
                {
                }
                else
                {
                    NavigateToTitle();
                }
            };
            option.OnBack += NavigateToTitle;

            stageSelect.OnStage += StageSelected;
            stageSelect.OnBack += NavigateToTitle;
        }

        /// <summary>
        /// InGameに遷移する
        /// </summary>
        /// <param name="nav">選んだステージ</param>
        private void StageSelected(StageNavigation nav)
        {
            Debug.Log("StageSelected : " + nav);
            if (!sceneChanger.LoadInGame(nav))
            {
                throw new NotImplementedException("not found navigation : " + nav);
            }
        }

        private void NavigateToTitle()
        {
            navigation.SetActive(true);
            navigation.SetScreen(TitleNavigation.Title);
        }

        private void OnCanceled()
        {
            switch (navigation.GetCurrentNav())
            {
                case TitleNavigation.Title:
                    NavigateToQuit();
                    break;
                case TitleNavigation.Option:
                    break;
                default:
                    NavigateToTitle();
                    break;
            }
        }


        private void NavigateToPlay()
        {
            data.Load();
            navigation.SetScreen(TitleNavigation.StageSelect);
            stageSelect.SetScores(data.GetStageData());
        }

        private void NavigateToCredit()
        {
            navigation.SetScreen(TitleNavigation.Credit);
        }

        private void NavigateToQuit()
        {
            navigation.SetScreen(TitleNavigation.Quit);
        }

        private void NavigateToOption()
        {
            navigation.SetScreen(TitleNavigation.Option);
            navigation.SetActive(false);
        }
    }
}