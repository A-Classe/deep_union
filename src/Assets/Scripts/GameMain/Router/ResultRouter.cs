using System.Collections.Generic;
using Core.Model.Scene;
using Core.Scenes;
using Core.User;
using Core.User.API;
using Core.Utility.UI.Navigation;
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

        private GameResult result;

        private readonly FirebaseAccessor db;

        [Inject]
        public ResultRouter(
            UserPreference userPreference,
            SceneChanger sceneChanger,
            ResultManager resultManager,
            FirebaseAccessor db
        )
        {
            this.userPreference = userPreference;

            this.sceneChanger = sceneChanger;

            this.resultManager = resultManager;

            this.db = db;

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
            userPreference.SetStageData((StageData.Stage)result.stageCode, result.GetScore());
            userPreference.Save();
            db.SetStageScore((StageData.Stage)result.stageCode, result.GetScore());
        }

        private void SetNavigation()
        {
            resultManager.OnNext += () =>
            {
                /* todo: 次のステージに飛ばす　*/
                var currentStage = (StageData.Stage)result.stageCode;
                if (currentStage != StageData.Stage.Stage1)
                {
                    navigation.SetActive(false);
                    sceneChanger.LoadInGame(currentStage + 1);
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

        private enum Nav
        {
            InResult
        }
    }
}