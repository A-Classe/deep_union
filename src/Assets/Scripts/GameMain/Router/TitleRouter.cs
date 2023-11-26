using System;
using System.Collections.Generic;
using Core.Scenes;
using Core.User;
using Core.Utility.UI.Navigation;
using Module.UI.Title.Credit;
using Module.UI.Title.Option;
using Module.UI.Title.Quit;
using Module.UI.Title.StageSelect;
using Module.UI.Title.Title;
using UnityEditor;
using VContainer;
using VContainer.Unity;

namespace GameMain.Router
{
    internal class TitleRouter : IStartable
    {
        private readonly CreditManager credit;

        private readonly UserPreference data;

        private readonly Navigation<TitleNavigation> navigation;


        private readonly OptionManager option;
        private readonly QuitManager quit;
        private readonly SceneChanger sceneChanger;
        private readonly StageSelectManager stageSelect;
        private readonly TitleManager title;

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

            var route = sceneChanger.GetTitle();
            navigation.SetActive(true);
            switch (route)
            {
                case TitleNavigation.StageSelect:
                    NavigateToPlay(true);
                    break;
                default:
                    NavigateToTitle();
                    break;
            }


            /* デバッグ用 */
            // data.Delete();
            data.Load();
        }


        private void SetNavigation()
        {
            navigation.OnCancel += _ => { OnCanceled(); };

            title.OnQuit += () => { NavigateToQuit(true); };
            title.OnOption += () => { NavigateToOption(true); };
            title.OnCredit += () => { NavigateToCredit(true); };
            title.OnPlay += () => { NavigateToPlay(true); };

            quit.OnClick += isQuit =>
            {
                if (isQuit)
                {
                    // TODO: ゲームを終了させる
#if UNITY_EDITOR
                    EditorApplication.isPlaying = false;
#else
                    UnityEngine.Application.Quit();
#endif
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
        ///     InGameに遷移する
        /// </summary>
        /// <param name="nav">選んだステージ</param>
        private void StageSelected(StageNavigation nav)
        {
            if (!sceneChanger.LoadInGame(nav.ToStage()))
                throw new NotImplementedException("not found navigation : " + nav);
            navigation.SetActive(false);
        }

        private void NavigateToTitle()
        {
            navigation.SetActive(true);
            navigation.SetScreen(TitleNavigation.Title, isReset: false);
        }

        private void OnCanceled()
        {
            switch (navigation.GetCurrentNav())
            {
                case TitleNavigation.Title:
                    NavigateToQuit(true);
                    break;
                case TitleNavigation.Option:
                    break;
                default:
                    NavigateToTitle();
                    break;
            }
        }


        private void NavigateToPlay(bool isReset)
        {
            data.Load();
            if (!data.GetIsFirst())
            {
                sceneChanger.LoadInGame(StageData.Stage.Tutorial);
            }
            else
            {
                navigation.SetScreen(TitleNavigation.StageSelect, isReset: isReset);
                stageSelect.SetScores(data.GetStageData());
            }
           
        }

        private void NavigateToCredit(bool isReset)
        {
            navigation.SetScreen(TitleNavigation.Credit, isReset: isReset);
        }

        private void NavigateToQuit(bool isReset)
        {
            navigation.SetScreen(TitleNavigation.Quit, isReset: isReset);
        }

        private void NavigateToOption(bool isReset)
        {
            navigation.SetScreen(TitleNavigation.Option, isReset: isReset);
            navigation.SetActive(false);
        }
    }
}