using System.Collections.Generic;
using Core.Model.Scene;
using Core.Scenes;
using Core.User;
using Core.Utility.UI.Navigation;
using UI.Result;
using VContainer;
using VContainer.Unity;

namespace GameMain
{
    public class ResultRouter: IStartable
    {
        private readonly UserPreference userPreference;

        private readonly SceneChanger sceneChanger;

        private readonly ResultManager resultManager;

        private readonly Navigation<Nav> navigation;

        private GameResult result;
        
        [Inject]
        public ResultRouter(
            UserPreference userPreference,
            SceneChanger sceneChanger,
            ResultManager resultManager
        )
        {
            this.userPreference = userPreference;

            this.sceneChanger = sceneChanger;

            this.resultManager = resultManager;
            
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
                navigation.SetScreen(Nav.InResult);
            });
            
            userPreference.Load();
            userPreference.SetStageData((StageData.Stage)result.stageCode, result.GetScore());
            userPreference.Save();
        }

        private void SetNavigation()
        {
            resultManager.OnNext += () =>
            {
                /* todo: 次のステージに飛ばす　*/
                var currentStage = (StageData.Stage)result.stageCode;
                if (currentStage != StageData.Stage.Stage5)
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
                if (currentStage != StageData.Stage.Stage5)
                {
                    navigation.SetActive(false);
                    sceneChanger.LoadInGame(currentStage);
                }
            };
        }
        
        private enum Nav
        {
            InResult
        }
    }
}