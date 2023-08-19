namespace Module.Task
{
    /// <summary>
    /// タスクにゲームループを適用するインターフェース
    /// </summary>
    public interface ITaskSystem
    {
        void Initialize();
        void Update(float deltaTime);
    }
}