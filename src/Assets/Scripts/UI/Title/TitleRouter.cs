using System.Collections.Generic;
using Core.Utility.UI.Navigation;
using Core.Utility.User;
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

        private readonly Navigation<Nav> navigation;


        private readonly OptionManager option;
        private readonly QuitManager quit;
        private readonly StageSelectManager stageSelect;
        private readonly TitleManager title;

        [Inject]
        public TitleRouter(
            TitleManager titleManager,
            QuitManager quitManager,
            OptionManager optionManager,
            CreditManager creditManager,
            StageSelectManager stageSelectManager,
            UserPreference dataManager
        )
        {
            title = titleManager;
            quit = quitManager;
            option = optionManager;
            credit = creditManager;
            stageSelect = stageSelectManager;

            data = dataManager;
            option.SetPreference(data);
            var initialManagers = new Dictionary<Nav, UIManager>
            {
                { Nav.Title, title },
                { Nav.Quit, quit },
                { Nav.Option, option },
                { Nav.Credit, credit },
                { Nav.StageSelect, stageSelect }
            };
            navigation = new Navigation<Nav>(initialManagers);
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

        private void StageSelected(StageSelectManager.Nav nav)
        {
            Debug.Log("StageSelected : " + nav);
        }

        private void NavigateToTitle()
        {
            navigation.SetActive(true);
            navigation.SetScreen(Nav.Title);
        }

        private void OnCanceled()
        {
            switch (navigation.GetCurrentNav())
            {
                case Nav.Title:
                    NavigateToQuit();
                    break;
                case Nav.Option:
                    break;
                default:
                    NavigateToTitle();
                    break;
            }
        }


        private void NavigateToPlay()
        {
            data.Load();
            navigation.SetScreen(Nav.StageSelect);
            stageSelect.SetScores(data.GetStageData());
        }

        private void NavigateToCredit()
        {
            navigation.SetScreen(Nav.Credit);
        }

        private void NavigateToQuit()
        {
            navigation.SetScreen(Nav.Quit);
        }

        private void NavigateToOption()
        {
            navigation.SetScreen(Nav.Option);
            navigation.SetActive(false);
        }

        private enum Nav
        {
            Title,
            Option,
            Quit,
            Credit,
            StageSelect
        }
    }
}