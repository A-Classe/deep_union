namespace Core.Model.User
{
    /// <summary>
    /// イベントの種類を定義
    /// </summary>
    public enum GameEventType
    {
        Default,
        
        // In Game Event
        Assign,
        Release,
        DelWorker,
        AddWorker,
        MovePlayer,
        MoveWorkers,
        
        // Sys Event
        GamePlay,
        GameClear,
        GameOver
    }
}