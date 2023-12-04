using Core.Input;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;

namespace Module.GameManagement
{
    public class TimeManager
    {
        private readonly InputActionMap inGameActionMap;

        [Inject]
        public TimeManager()
        {
            inGameActionMap = InputActionProvider.Instance.GetActionMap(ActionGuid.InGame.MapId);
        }

        public void Pause()
        {
            Time.timeScale = 0;
            inGameActionMap.Disable();
        }

        public void Resume()
        {
            return;
            Time.timeScale = 1;
            inGameActionMap.Enable();
        }
    }
}