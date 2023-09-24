using System.Collections.Generic;
using Core.Utility.UI.Navigation;
using GameMain.UI.Screen.GameOver;
using GameMain.UI.Screen.InGame;
using UI.Title.Option;
using UnityEngine;

namespace GameMain.UI
{
    public class GameUIManager: MonoBehaviour
    {
        [SerializeField] private InGameManager inGameManager;
        [SerializeField] private GameOverManager gameOverManager;
        [SerializeField] private OptionManager optionManager;
        private readonly Navigation<InGameNav> navigation;

        public GameUIManager()
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