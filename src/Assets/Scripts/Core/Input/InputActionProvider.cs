using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Core.Input
{
    /// <summary>
    /// 登録解除不要なInputActionを提供するクラス
    /// </summary>
    public class InputActionProvider : SingletonMonoBehaviour<InputActionProvider>
    {
        [SerializeField] private InputActionAsset inputActionAsset;
        private readonly List<InputEvent> inputEvents = new List<InputEvent>();

        private void Start()
        {
            inputActionAsset.Enable();
        }

        public InputEvent CreateEvent(Guid guid)
        {
            InputAction inputAction = inputActionAsset.FindAction(guid);
            Debug.Assert(inputAction != null);

            InputEvent inputEvent = new InputEvent(inputAction);
            inputEvents.Add(inputEvent);

            return inputEvent;
        }

        private void OnDestroy()
        {
            foreach (InputEvent inputEvent in inputEvents)
            {
                inputEvent.Clear();
            }
        }
    }
}