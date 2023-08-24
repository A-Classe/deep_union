namespace Module.Task
{
    /// <summary>
    ///     タスクにゲームループを適用するインターフェース
    /// </summary>
    public interface ITaskSystem
    {
        TaskState State { get; }

        void Initialize();
        void TaskSystemUpdate(float deltaTime);
    }
}