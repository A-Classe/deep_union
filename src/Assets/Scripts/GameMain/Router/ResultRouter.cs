using System;
using System.Collections.Generic;
using Core.Model.Scene;
using Core.Scenes;
using Core.User;
using Core.User.API;
using Core.Utility.UI.Navigation;
using Module.Extension.UI;
using Module.UI.Result;
using VContainer;
using VContainer.Unity;

namespace GameMain.Router
{
    public class ResultRouter : IStartable
    {
        private readonly Navigation<Nav> navigation;

        private readonly ResultManager resultManager;

        private readonly SceneChanger sceneChanger;
        private readonly UserPreference userPreference;

        private readonly VideoPlayerControllerExt videoPlayer;

        private GameResult result;

        private readonly FirebaseAccessor db;

        [Inject]
        public ResultRouter(
            UserPreference userPreference,
            SceneChanger sceneChanger,
            ResultManager resultManager,
            FirebaseAccessor db,
            VideoPlayerControllerExt videoPlayer
        )
        {
            this.userPreference = userPreference;

            this.sceneChanger = sceneChanger;

            this.resultManager = resultManager;

            this.db = db;

            this.videoPlayer = videoPlayer;

            // setup navigation
            navigation = new Navigation<Nav>(
                new Dictionary<Nav, UIManager>
                {
                    { Nav.InResult, resultManager }
                }
            );
        }

        public void Start()
        {
            SetNavigation();
            result = sceneChanger.GetResultParam();
            resultManager.SetScore(result, () =>
            {
                navigation.SetActive(true);
                navigation.SetScreen(Nav.InResult, false);
            });

            userPreference.Load();
            uint score = result.GetScore();
            StageData.Stage stage = (StageData.Stage)result.stageCode;
            // スコアが高ければ or スコアがなければ更新
            if (userPreference.GetStageData().TryGetValue(stage, out uint currentScore))
            {
                if (currentScore < score)
                {
                    userPreference.SetStageData(stage, score);
                    userPreference.Save();
                    db.SetStageScore(stage, score);
                }
            }
            else
            {
                userPreference.SetStageData(stage, score);
                userPreference.Save();
                db.SetStageScore(stage, score);
            }
            videoPlayer.Play();
        }

        private void SetNavigation()
        {
            resultManager.OnNext += () =>
            {
                var currentStage = (StageData.Stage)result.stageCode;
                if (currentStage != StageData.Stage.Stage3)
                {
                    navigation.SetActive(false);
                    StageData.Stage next = currentStage + 1;
                    ToStage(next);
                }
            };
            resultManager.OnSelect += () =>
            {
                /* todo: ステージセレクトに直接遷移させる　*/
                navigation.SetActive(false);
                sceneChanger.LoadTitle(TitleNavigation.StageSelect);
            };

            resultManager.OnRetry += () =>
            {
                var currentStage = (StageData.Stage)result.stageCode;
                navigation.SetActive(false);
                sceneChanger.LoadInGame(currentStage);
            };
        }

        private void ToStage(StageData.Stage stage)
        {
            if (stage == StageData.Stage.Tutorial)
            {
                sceneChanger.LoadInGame(StageData.Stage.Tutorial);
            }
            else if (!sceneChanger.LoadBeforeMovieInGame(stage))
            {
                throw new NotImplementedException("not found navigation : " + stage);
            }
        }

        private enum Nav
        {
            InResult
        }
    }
}