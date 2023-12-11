using System;
using Core.Input;
using UnityEngine.InputSystem;
using VContainer;

namespace Debug
{
    public class DebugGameClear
    {
        private readonly InputEvent gameClearEvent;
        public event Action OnGameClearRequested;

        [Inject]
        public DebugGameClear()
        {
            gameClearEvent = InputActionProvider.Instance.CreateEvent(ActionGuid.DebugKey.GameClear);
            gameClearEvent.Started += GameClear;
        }

        private void GameClear(InputAction.CallbackContext obj)
        {
            OnGameClearRequested?.Invoke();
        }
    }
}