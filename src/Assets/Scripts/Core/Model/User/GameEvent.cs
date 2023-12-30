using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Model.User
{
    public class GameEvent
    {
        public GameEventType EventType { get; private set; }
        private Dictionary<string, object> Parameters { get; set; }

        public GameEvent(GameEventType eventType, Dictionary<string, object> parameters = null)
        {
            EventType = eventType;
            Parameters = parameters ?? new Dictionary<string, object>();
        }

        public string Serialize()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append(EventType.ToString());
            stringBuilder.Append("|");
            foreach (var pair in Parameters)
            {
                stringBuilder.Append($"{pair.Key}={pair.Value};");
            }
            return stringBuilder.ToString();
        }

        public static GameEvent Deserialize(string serializedEvent)
        {
            var parts = serializedEvent.Split('|');
            var eventType = (GameEventType)Enum.Parse(typeof(GameEventType), parts[0]);
            var parameters = new Dictionary<string, object>();
            var paramParts = parts[1].Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var part in paramParts)
            {
                var keyValuePair = part.Split('=');
                parameters[keyValuePair[0]] = keyValuePair[1];
            }
            return new GameEvent(eventType, parameters);
        }
        
        public void SetParameter(Dictionary<string, object> param)
        {
            Parameters = param;
        }

        public object GetParameter(string key)
        {
            return Parameters.GetValueOrDefault(key);
        }
        
        public Dictionary<string, object> GetParameters()
        {
            return Parameters;
        }
    }
}