namespace Module.Player.State
{
    /// <summary>
    ///     プレイヤーの状態
    /// </summary>
    public enum PlayerState
    {
        /// <summary>
        ///     システム上の停止
        /// </summary>
        Pause,
            
        /// <summary>
        ///     ゲーム上の停止
        /// </summary>
        Stop,
            
        /// <summary>
        ///     ピンに追従する
        /// </summary>
        FollowToPin,

        /// <summary>
        ///     自動航行
        /// </summary>
        Auto,
    }
}