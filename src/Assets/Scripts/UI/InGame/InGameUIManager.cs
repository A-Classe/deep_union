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
                sceneChanger.LoadInGame(sceneChanger.GetInGame());
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

        public void UpdateStageProgress(int value)
        {
            inGameManager.SetStageProgress((uint)value);
        }
        
        public void SetHP(short current, short? max = null)
        {
            if (max.HasValue)
            {
                inGameManager.SetHP((uint)current, (uint)max.Value);
            }
            else
            {
                inGameManager.SetHP((uint)current);   
            }
        }
        
        public void SetWorkerCount(uint value)
        {
            inGameManager.SetWorkerCount(value);
        }
        
        public void SetResourceCount(uint current, uint? max = null)
        {
            inGameManager.SetResource(current, max);
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