using System;
using System.Threading;
using Core.Input;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using Wanna.DebugEx;

namespace Module.Player.Controller
{
    public class FollowPin : MonoBehaviour
    {
        [SerializeField] private Transform pinOrigin;
        [SerializeField] private float pinDuration;
        [SerializeField] private LayerMask groundLayer;

        public event Action OnArrived;
        public event Action OnPinned;
        private InputEvent pinEvent;
        private Vector3 pinPosition;
        private bool isPinning;
        private CancellationTokenSource combinedCanceller;

        private void Start()
        {
            pinEvent = InputActionProvider.Instance.CreateEvent(ActionGuid.InGame.Pin);
            pinEvent.Started += OnPinPushed;
            pinEvent.Canceled += OnPinReleased;
        }

        private void OnPinPushed(InputAction.CallbackContext obj)
        {
            if (isPinning)
                return;

            isPinning = true;
            SequencePin().Forget();
        }

        private void OnPinReleased(InputAction.CallbackContext obj)
        {
            if (!isPinning)
                return;

            isPinning = false;
        }

        private async UniTaskVoid SequencePin()
        {
            CancellationToken destroyCanceller = this.GetCancellationTokenOnDestroy();
            DateTime end = DateTime.Now + TimeSpan.FromSeconds(pinDuration);

            //ピンをキャンセルした or 一定時間を超えた場合
            await UniTask.WaitUntil(() => !isPinning || DateTime.Now >= end, cancellationToken: destroyCanceller);

            if (isPinning)
            {
                isPinning = false;
                DetectPinPosition();
            }
        }

        private void DetectPinPosition()
        {
            if (Physics.Raycast(pinOrigin.position, Vector3.down, out RaycastHit hit, 100f, groundLayer))
            {
                pinPosition = hit.point;
                OnPinned?.Invoke();
                DebugEx.Log("ピンしました");
                DebugEx.Log(pinPosition);
            }
        }

        public void ArriveToPin()
        {
            OnArrived?.Invoke();
        }

        public Vector3 GetPosition()
        {
            return pinPosition;
        }
    }
}