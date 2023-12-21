using System.Collections.Generic;

namespace Core.Model.User
{
    public class GameEvent
    {
        public GameEventType EventType { get; private set; }
        private Dictionary<string, object> parameters;

        public GameEvent(GameEventType eventType)
        {
            EventType = eventType;
            parameters = new Dictionary<string, object>();
        }

        public void AddParameter(string key, object value)
        {
            parameters[key] = value;
        }

        public void SetParameter(Dictionary<string, object> param)
        {
            parameters = param;
        }

        public object GetParameter(string key)
        {
            if (parameters.TryGetValue(key, out object value))
            {
                return value;
            }

            return null;
        }

        public Dictionary<string, object> GetParameters()
        {
            return parameters;
        }
    }
}