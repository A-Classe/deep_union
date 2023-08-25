namespace Module.Player.State
{
    /// <summary>
    /// プレイヤーの状態
    /// </summary>
    public enum PlayerState
    {
        /// <summary>
        /// 進行中の停止状態
        /// </summary>
        Wait,
        /// <summary>
        /// プレイ中
        /// </summary>
        Go,
        /// <summary>
        /// システム上の停止
        /// </summary>
        Pause
    }
}