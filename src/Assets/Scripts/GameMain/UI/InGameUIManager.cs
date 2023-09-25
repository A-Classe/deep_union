using System;
using System.Collections.Generic;
using Core.Utility.UI.Navigation;
using GameMain.UI.Screen.GameOver;
using GameMain.UI.Screen.InGame;
using UI.Title.Option;
using UnityEngine;

namespace GameMain.UI
{
    public class InGameUIManager: MonoBehaviour
    {
        [SerializeField] private InGameManager inGameManager;
        [SerializeField] private GameOverManager gameOverManager;
        [SerializeField] private OptionManager optionManager;
        private Navigation<InGameNav> navigation;

        public InGameUIManager() { }

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

        public void SetScreen(InGameNav nav)
        {
            navigation.SetScreen(nav);
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