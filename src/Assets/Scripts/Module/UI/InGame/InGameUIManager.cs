using System;
using System.Collections.Generic;
using Core.Model.Minimap;
using Core.Scenes;
using Core.User;
using Core.User.Recorder;
using Core.Utility.UI.Navigation;
using Module.GameSetting;
using Module.UI.InGame.GameOver;
using Module.UI.InGame.InGame;
using Module.UI.InGame.Pause;
using Module.UI.Title.Option;
using UnityEngine;

namespace Module.UI.InGame
{
    public class InGameUIManager : MonoBehaviour
    {
        [SerializeField] private InGameManager inGameManager;
        [SerializeField] private GameOverManager gameOverManager;
        [SerializeField] private PauseManager pauseManager;
        [SerializeField] private OptionManager optionManager;
        private Navigation<InGameNav> navigation;

        private SceneChanger sceneChanger;

        public event Action OnGameInactive;
        public event Action OnGameActive;
        
        private enum State
        {
            Main,
            Minimap
        }
        private BrightController brightController;

        public event Action OnNeedReport;


        private State state = State.Main;
        
        private void Awake()
        {
            navigation = new Navigation<InGameNav>(
                new Dictionary<InGameNav, UIManager>
                {
                    { InGameNav.InGame, inGameManager },
                    { InGameNav.GameOver, gameOverManager },
                    { InGameNav.Option, optionManager },
                    { InGameNav.Pause, pauseManager }
                }
            );
        }

        private void Start()
        {
            gameOverManager.OnSelect += () =>
            {
                OnNeedReport?.Invoke();
                navigation.SetActive(false);
                sceneChanger.LoadTitle(TitleNavigation.StageSelect);
            };

            gameOverManager.OnRetry += () =>
            {
                navigation.SetActive(false);
                sceneChanger.LoadInGame(sceneChanger.GetInGame());
            };

            optionManager.OnBack += () =>
            {
                navigation.SetActive(true);
                navigation.SetScreen(InGameNav.Pause);
            };

            pauseManager.OnClick += nav =>
            {
                switch (nav)
                {
                    case PauseManager.Nav.Resume:
                        navigation.SetActive(true);
                        navigation.SetScreen(InGameNav.InGame);
                        OnGameActive?.Invoke();
                        break;
                    case PauseManager.Nav.Option:
                        navigation.SetScreen(InGameNav.Option, isReset: true);
                        navigation.SetActive(false);
                        OnGameInactive?.Invoke();
                        break;
                    case PauseManager.Nav.Restart:
                        OnGameActive?.Invoke();
                        OnNeedReport?.Invoke();
                        navigation.SetActive(false);
                        sceneChanger.LoadInGame(sceneChanger.GetInGame());
                        break;
                    case PauseManager.Nav.Exit:
                        OnGameActive?.Invoke();
                        OnNeedReport?.Invoke();
                        navigation.SetActive(false);
                        sceneChanger.LoadTitle(TitleNavigation.StageSelect);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(nav), nav, null);
                }
            };

            inGameManager.OnMinimapClick += () =>
            {
                inGameManager.SetMiniView();
                navigation.SetActive(true);
                state = State.Main;
                OnGameActive?.Invoke();
            };

            optionManager.OnBrightness += value =>
            {
                brightController.SetBrightness(value / 10f);
            };

            navigation.OnCancel += _ =>
            {
                Debug.Log("cancel");
            };
            
            navigation.SetActive(true);
        }


        public void StartGame(UserPreference preference, AudioMixerController controller)
        {
            optionManager.SetPreference(preference, controller);
            navigation.SetActive(true);
            navigation.SetScreen(InGameNav.InGame);
        }

        public void StartPause()
        {
            navigation.SetActive(true);
            navigation.SetScreen(InGameNav.Pause, isReset: true);
            OnGameInactive?.Invoke();
        }

        public void SetGameOver()
        {
            navigation.SetScreen(InGameNav.GameOver, isReset: true);
            OnGameInactive?.Invoke();
        }

        public void StartMinimap()
        {
            if (state == State.Main)
            {
                state = State.Minimap;
                inGameManager.SetFocusView();
                OnGameInactive?.Invoke();
            }
        }

        public void SetSceneChanger(SceneChanger scene)
        {
            sceneChanger = scene;
        }

        public void UpdateStageProgress(int value)
        {
            inGameManager.SetStageProgress((uint)value);
        }

        public void SetHp(short current, short? max = null)
        {
            if (max.HasValue)
                inGameManager.SetHp((uint)current, (uint)max.Value);
            else
                inGameManager.SetHp((uint)current);
        }

        public void SetWorkerCount(uint value, uint? max = null)
        {
            inGameManager.SetWorkerCount(value, max);
        }

        public void SetResourceCount(uint current, uint? max = null)
        {
            inGameManager.SetResource(current, max);
        }

        public void SetMinimapParam(MiniMapBuild data)
        {
            inGameManager.SetMinimapParam(data);
        }

        
        public void SetBrightnessController(BrightController controller)
        {
            brightController = controller;
        }
    }

    public enum InGameNav
    {
        /**
         Tutorial*** ...
         */
        GameOver,
        Option,
        Pause,
        InGame
    }
}