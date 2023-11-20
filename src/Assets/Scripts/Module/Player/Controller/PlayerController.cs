using System;
using GameMain.Presenter;
using Module.Player.State;
using UnityEngine;

namespace Module.Player.Controller
{
    [Serializable]
    public class MovementSetting
    {
        public float MoveResistance;
        public float RotateResistance;
    }

    /// <summary>
    ///     プレイヤーの操作に関するクラス
    /// </summary>
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private Rigidbody rig;
        [SerializeField] private MovementSetting setting;
        private IPlayerState currentState;
        [NonSerialized] public GameParam gameParam;
        private Vector3? startPosition;

        private IPlayerState[] states;

        private void FixedUpdate()
        {
            StateUpdate();
        }

        public event Action<PlayerState> OnStateChanged;

        /// <summary>
        ///     objectの初期化
        /// </summary>
        private void Initialize()
        {
            states = new IPlayerState[]
            {
                new WaitState(),
                new GoState(this, rig, setting),
                new PauseState(rig)
            };

            SetState(PlayerState.Pause);
        }

        /// <summary>
        ///     移動に関するupdate
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

        public void InitParam(GameParam gameParam)
        {
            this.gameParam = gameParam;
            Initialize();
        }

        /// <summary>
        ///     ゲーム開始時に実行する
        /// </summary>
        public void PlayerStart()
        {
            if (startPosition.HasValue) transform.position = startPosition.Value;
        }

        /// <summary>
        ///     ステータスの取得
        /// </summary>
        /// <returns>Playerの状態を返す</returns>
        public PlayerState GetState()
        {
            return currentState.GetState();
        }
    }
}