using VContainer;

namespace Module.Task
{
    /// <summary>
    ///     タスクにゲームループを適用するインターフェース
    /// </summary>
    public interface ITaskSystem
    {
        TaskState State { get; }

        void Initialize(IObjectResolver container);
        void TaskSystemUpdate(float deltaTime);
    }
}