using System;
using System.Collections.Generic;
using Core.Model.User;
using UnityEngine;

namespace Core.User.Recorder
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class GameActionRecorder
    {
        private readonly LinkedList<GameEvent> events = new();

        private readonly bool printLog = false;

        public event Action<GameEvent> OnAnyEvent;

        public void LogEvent(GameEvent gameEvent)
        {
            events.AddLast(gameEvent);
            OnAnyEvent?.Invoke(gameEvent);
            if (printLog)
            {
                string value = "";
                foreach (var keyValuePair in gameEvent.GetParameters())
                {
                    value += "{ " + keyValuePair.Key + ":: " + keyValuePair.Value + "}, ";
                }

                Debug.Log("==== " + gameEvent.EventType + " ====\n" + value + "\n" + "==== " + gameEvent.EventType + " ====");
            }
        }

        public Report GenerateReport()
        {
            return Report.GenerateToEvents(events);
        }

        public void Clear()
        {
            events.Clear();
        }

        // 必要に応じてその他のメソッド
    }
}