using System;
using System.Linq;
using GameMain.Presenter;
using Module.Player.State;
using UnityEngine;
using UnityEngine.AI;
using Wanna.DebugEx;

namespace Module.Player.Controller
{
    /// <summary>
    ///     プレイヤーの操作に関するクラス
    /// </summary>
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private Rigidbody rig;
        [SerializeField] private NavMeshAgent navMeshAgent;
        [SerializeField] private NavMeshObstacle navMeshObstacle;
        [SerializeField] private FollowPin followPin;

        [Header("ピンを追従してない状態の移動設定")]
        [SerializeField]
        private MovementSetting setting;

        private IPlayerState currentState;

        private IPlayerState[] states;

        public event Action<PlayerState> OnStateChanged;
        
        public event Action<float> OnMoveDistance; 
        
        private Vector3 lastPosition = Vector3.zero;

        /// <summary>
        ///     objectの初期化
        /// </summary>
        private void Awake()
        {
            followPin.OnArrived += () => SetState(PlayerState.Auto);
            followPin.OnPinned += () => SetState(PlayerState.FollowToPin);

            states = new IPlayerState[]
            {
                new PauseState(rig, setting),
                new AutoState(rig, setting),
                new FollowToPinState(rig, navMeshAgent, navMeshObstacle, followPin)
            };

            SetState(PlayerState.Pause);
        }

        private void FixedUpdate()
        {
            StateUpdate();
            SendLog();
        }

        /// <summary>
        ///     移動に関するupdate
        /// </summary>
        private void StateUpdate()
        {
            currentState.FixedUpdate();
        }

        public void SetState(PlayerState state)
        {
            try
            {
                currentState?.Stop();
                currentState = states.First(item => item.GetState() == state);
                currentState.Start();
                OnStateChanged?.Invoke(state);
            }
            catch (Exception e)
            {
                DebugEx.LogError("ステートが存在しません!");
                DebugEx.LogException(e);
                throw;
            }
        }

        /// <summary>
        ///     ステータスの取得
        /// </summary>
        /// <returns>Playerの状態を返す</returns>
        public PlayerState GetState()
        {
            return currentState.GetState();
        }
        
        
        private void SendLog()
        {
            if (lastPosition != Vector3.zero)
            {
                float distance = Vector3.Distance(lastPosition, transform.position);
                if (Math.Abs(distance) < 0.001f) return;
                OnMoveDistance?.Invoke(distance);
            }
            lastPosition = transform.position;
        }
    }
}