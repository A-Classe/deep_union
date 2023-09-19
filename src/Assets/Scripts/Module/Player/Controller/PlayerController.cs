using System;
using Core.Model.Player;
using Module.Player.State;
using UnityEngine;

namespace Module.Player.Controller
{
    /// <summary>
    /// プレイヤーの操作に関するクラス
    /// </summary>
    public class PlayerController : MonoBehaviour
    {
        private Vector3? startPosition;
        private float speed = 1f;

        public float Speed => speed;

        private IPlayerState currentState;

        private IPlayerState[] states;
        public event Action<PlayerState> OnStateChanged;

        private void Awake()
        {
            Initialize();
        }


        private void FixedUpdate()
        {
            StateUpdate();
        }

        /// <summary>
        /// objectの初期化
        /// </summary>
        private void Initialize()
        {
            states = new IPlayerState[]
            {
                new WaitState(),
                new GoState(this),
                new PauseState()
            };

            SetState(PlayerState.Pause);
        }

        /// <summary>
        /// 移動に関するupdate
        /// </summary>
        private void StateUpdate()
        {
            currentState.Update();
        }

        public void SetState(PlayerState state)
        {
            currentState = state switch
            {
                PlayerState.Wait => states[0],
                PlayerState.Go => states[1],
                PlayerState.Pause => states[2],
                _ => null
            };

            OnStateChanged?.Invoke(state);
        }

        public void InitParam(PlayerInitModel model)
        {
            speed = model.speed ?? 1f;
            startPosition = model.startPosition;
        }

        /// <summary>
        /// ゲーム開始時に実行する
        /// </summary>
        public void PlayerStart()
        {
            if (startPosition.HasValue)
            {
                transform.position = startPosition.Value;
            }
        }

        /// <summary>
        /// ステータスの取得
        /// </summary>
        /// <returns>Playerの状態を返す</returns>
        public PlayerState GetState() => currentState.GetState();
    }
}