using System;
using System.Collections.Generic;
using Core.Scenes;
using Core.User;
using Core.User.API;
using Core.Utility.UI.Navigation;
using Module.GameSetting;
using Module.UI.Title.Credit;
using Module.UI.Title.Option;
using Module.UI.Title.Quit;
using Module.UI.Title.Ranking;
using Module.UI.Title.StageSelect;
using Module.UI.Title.Stats;
using Module.UI.Title.Title;
using UnityEditor;
using VContainer;
using VContainer.Unity;

namespace GameMain.Router
{
    internal class TitleRouter : IStartable
    {
        private readonly UserPreference data;

        private readonly Navigation<TitleNavigation> navigation;


        private readonly OptionManager option;
        private readonly QuitManager quit;
        private readonly SceneChanger sceneChanger;
        private readonly StageSelectManager stageSelect;
        private readonly TitleManager title;
        private readonly StatsManager statsManager;
        private readonly FirebaseAccessor accessor;
        private readonly RankingManager rankingManager;
        

        [Inject]
        public TitleRouter(
            TitleManager titleManager,
            QuitManager quitManager,
            OptionManager optionManager,
            CreditManager creditManager,
            StageSelectManager stageSelectManager,
            StatsManager statsManager,
            UserPreference dataManager,
            SceneChanger sceneChanger,
            AudioMixerController audioMixerController,
            FirebaseAccessor firebaseAccessor,
            RankingManager rankingManager
        )
        {
            title = titleManager;
            quit = quitManager;
            option = optionManager;
            stageSelect = stageSelectManager;
            this.sceneChanger = sceneChanger;
            this.statsManager = statsManager;
            accessor = firebaseAccessor;
            this.rankingManager = rankingManager;

            data = dataManager;
            option.SetPreference(data, audioMixerController);
            UserData userData = data.GetUserData();
            audioMixerController.SetMasterVolume(userData.masterVolume.value / 10f);
            audioMixerController.SetBGMVolume(userData.musicVolume.value / 10f);
            audioMixerController.SetSEVolume(userData.effectVolume.value / 10f);
            
            var initialManagers = new Dictionary<TitleNavigation, UIManager>
            {
                { TitleNavigation.Title, title },
                { TitleNavigation.Quit, quit },
                { TitleNavigation.Option, option },
                { TitleNavigation.Credit, creditManager },
                { TitleNavigation.StageSelect, stageSelect },
                { TitleNavigation.Stats, statsManager },
                { TitleNavigation.Ranking, rankingManager }
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
            rankingManager.OnChangedName += name =>
            {
                UnityEngine.Debug.Log(name);
                accessor.SetName(name);
                UserData userData = data.GetUserData();
                userData.name.value = name;
                data.SetUserData(userData);
                data.Save();
                ReloadRanking();
            };
        }

        private void ReloadRanking()
        {
            data.Load();
            UserData userData = data.GetUserData();
                
            rankingManager.SetName(userData.name.value);
            accessor.GetAllData((ranking) =>
            {
                rankingManager.SetRanking(ranking, data.GetUserData().uuid.value);
                rankingManager.Reload();
            });
        }


        private void SetNavigation()
        {
            navigation.OnCancel += _ => { OnCanceled(); };

            title.OnQuit += () => { NavigateToQuit(true); };
            title.OnOption += () => { NavigateToOption(true); };
            title.OnCredit += () => { NavigateToCredit(true); };
            title.OnPlay += () => { NavigateToPlay(true); };
            title.OnStats += () => { NavigateToStats(); };

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
            stageSelect.OnRanking += NavigateToRanking;
            statsManager.OnBack += NavigateToTitle;
            rankingManager.OnBack += () =>
            {
                NavigateToPlay(false);
            };
        }

        /// <summary>
        ///     InGameに遷移する
        /// </summary>
        /// <param name="nav">選んだステージ</param>
        private void StageSelected(StageNavigation nav)
        {
            navigation.SetScreen(TitleNavigation.Title, isAnimate: false);
            if (!sceneChanger.LoadBeforeMovieInGame(nav.ToStage()))
            {
                throw new NotImplementedException("not found navigation : " + nav);
            }
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

        private void NavigateToStats()
        {
            statsManager.SetReports(data.GetReport());
            navigation.SetScreen(TitleNavigation.Stats);
        }

        private void NavigateToQuit(bool isReset)
        {
            navigation.SetScreen(TitleNavigation.Quit, isReset: isReset);
        }

        private void NavigateToRanking(StageData.Stage stage)
        {
            rankingManager.SetStage(stage);
            ReloadRanking();
            rankingManager.Reload();
            navigation.SetScreen(TitleNavigation.Ranking);
        }

        private void NavigateToOption(bool isReset)
        {
            navigation.SetScreen(TitleNavigation.Option, isReset: isReset);
            navigation.SetActive(false);
        }
    }
}