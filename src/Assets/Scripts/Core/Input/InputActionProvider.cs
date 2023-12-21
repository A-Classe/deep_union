using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Core.Input
{
    /// <summary>
    ///     登録解除不要なInputActionを提供するクラス
    /// </summary>
    public class InputActionProvider : SingletonMonoBehaviour<InputActionProvider>
    {
        [SerializeField] private InputActionAsset inputActionAsset;
        private readonly List<InputEvent> inputEvents = new();

        private void Start()
        {
            inputActionAsset.Enable();
        }

        private void OnDestroy()
        {
            foreach (var inputEvent in inputEvents)
            {
                inputEvent.Clear();
            }
        }

        public InputEvent CreateEvent(Guid guid)
        {
            var inputAction = inputActionAsset.FindAction(guid);
            Debug.Assert(inputAction != null);

            var inputEvent = new InputEvent(inputAction);
            inputEvents.Add(inputEvent);

            return inputEvent;
        }

        public InputActionMap GetActionMap(Guid guid)
        {
            return inputActionAsset.FindActionMap(guid);
        }
    }
}