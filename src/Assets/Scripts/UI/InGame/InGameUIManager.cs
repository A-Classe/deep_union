using System.Collections.Generic;
using Core.Input;
using Core.Model.Scene;
using Core.Scenes;
using Core.Utility.UI.Navigation;
using UI.InGame.Screen.GameOver;
using UI.InGame.Screen.InGame;
using UI.Title.Option;
using UnityEngine;

namespace UI.InGame
{
    public class InGameUIManager: MonoBehaviour
    {
        [SerializeField] private InGameManager inGameManager;
        [SerializeField] private GameOverManager gameOverManager;
        [SerializeField] private OptionManager optionManager;
        private Navigation<InGameNav> navigation;

        private SceneChanger sceneChanger;

        private void Awake()
        {
            navigation = new Navigation<InGameNav>(
                new Dictionary<InGameNav, UIManager>
                {
                    { InGameNav.InGame, inGameManager },
                    { InGameNav.GameOver, gameOverManager },
                    { InGameNav.Option, optionManager }
                }
            );
        }

        private void Start()
        {
            gameOverManager.OnSelect += () =>
            {
                navigation.SetActive(false);
                sceneChanger.LoadTitle(TitleNavigation.StageSelect);
            };

            gameOverManager.OnRetry += () =>
            {
                navigation.SetActive(false);
                /* todo: テスト用にretryでresultに遷移 */
                sceneChanger.LoadResult(new GameResult
                {
                    WorkerCount = 22,
                    Hp = 50,
                    Resource = 45,
                    stageCode = (int)sceneChanger.GetInGame()
                });
            };
            
            /*todo: 以下テスト用 */
            var escEvent = InputActionProvider.Instance.CreateEvent(ActionGuid.InGame.ESC);
            escEvent.Started += _ =>
            {
                navigation.SetScreen(InGameNav.GameOver);
            };
            
            navigation.SetActive(true);
        }

        public void SetScreen(InGameNav nav)
        {
            navigation.SetScreen(nav);
        }

        public void SetSceneChanger(SceneChanger scene)
        {
            sceneChanger = scene;
        }
    }
    
    public enum InGameNav {
        /**
         Tutorial*** ...
         */
        
        GameOver,
        Option,
        InGame,
    }
}