using Core.Model.User;
using VContainer;

namespace Core.User.Recorder
{
    /// <summary>
    ///     ゲーム内イベント送信するクラス
    /// </summary>
    public class EventBroker
    {
        private readonly GameActionRecorder recorder;

        [Inject]
        public EventBroker(
            GameActionRecorder recorder
        )
        {
            this.recorder = recorder;
        }

        public void SendEvent(GameEvent @event)
        {
            recorder.LogEvent(@event);
        }

        public void Clear()
        {
            recorder.Clear();
        }
    }
}