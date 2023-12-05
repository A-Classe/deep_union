using System;
using System.Threading;
using Core.Input;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using Wanna.DebugEx;

namespace Module.Player.Controller
{
    /// <summary>
    /// 潜水艦を誘導するピンを操作するクラス
    /// </summary>
    public class FollowPin : MonoBehaviour
    {
        [SerializeField] private Transform pinOrigin;
        [SerializeField] private Transform pinObject;
        [SerializeField] private Transform pinAssist;
        [SerializeField] private float pinDuration;
        [SerializeField] private Vector3 pinOffset;
        [SerializeField] private LayerMask groundLayer;

        /// <summary>
        /// 潜水艦がピンに到達したときのイベント
        /// </summary>
        public event Action OnArrived;

        /// <summary>
        /// ピンを差したときのイベント
        /// </summary>
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

            pinObject.gameObject.SetActive(false);
            pinAssist.gameObject.SetActive(false);
        }

        private void OnPinPushed(InputAction.CallbackContext obj)
        {
            if (isPinning)
                return;

            //長押し待機
            isPinning = true;
            pinAssist.gameObject.SetActive(true);
            SequencePin().Forget();
        }

        private void OnPinReleased(InputAction.CallbackContext obj)
        {
            if (!isPinning)
                return;

            isPinning = false;
            pinAssist.gameObject.SetActive(false);
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
                bool isHit = DetectPinPosition();

                if (isHit)
                {
                    pinAssist.gameObject.SetActive(false);

                    pinObject.SetParent(null);
                    pinObject.position = pinOrigin.position + pinOffset;
                    pinObject.gameObject.SetActive(true);
                }
            }
        }

        private bool DetectPinPosition()
        {
            bool isHit = Physics.Raycast(pinOrigin.position, Vector3.down, out RaycastHit hitInfo, 100f, groundLayer);

            //真下にレイを飛ばして判定
            if (isHit)
            {
                //真下の点から一番近いNavMeshの点を取得
                NavMesh.SamplePosition(hitInfo.point, out NavMeshHit sampleInfo, 100f, NavMesh.AllAreas);

                pinPosition = sampleInfo.position;
                OnPinned?.Invoke();
            }

            return isHit;
        }

        public void ArriveToPin()
        {
            pinObject.gameObject.SetActive(false);
            OnArrived?.Invoke();
        }

        /// <summary>
        /// ピンが指している位置を返します
        /// </summary>
        /// <returns>ピンが指している位置</returns>
        public Vector3 GetPosition()
        {
            return pinPosition;
        }
    }
}