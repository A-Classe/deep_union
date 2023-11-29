using System;
using Core.Input;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Module.Player.Controller
{
    public class FollowPin : MonoBehaviour
    {
        public event Action OnArrived;
        private InputEvent pinEvent;
        private Vector3 pinPosition;
        
        private void Start()
        {
            pinEvent = InputActionProvider.Instance.CreateEvent(ActionGuid.InGame.Pin);
            pinEvent.Started += OnPinClicked;
        }

        private void OnPinClicked(InputAction.CallbackContext obj)
        {
            
        }

        public void ArriveToPin()
        {
            OnArrived?.Invoke();
        }
        
        public Vector3 GetPosition()
        {
            return Vector3.zero;
        }
    }
}