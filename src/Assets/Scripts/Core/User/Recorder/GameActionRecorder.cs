using System;
using System.Collections.Generic;
using System.Linq;
using Core.Model.User;
using UnityEngine;

namespace Core.User.Recorder
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class GameActionRecorder
    {
        private readonly LinkedList<string> events = new();

        private readonly bool printLog = false;

        public event Action<GameEvent> OnAnyEvent;

        public void AddEvent(GameEvent gameEvent)
        {
            events.AddLast(gameEvent.Serialize());
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
            var evs = events.Select(GameEvent.Deserialize).ToList();
            var gameEventsLinkedList = new LinkedList<GameEvent>(evs);
            return Report.GenerateToEvents(gameEventsLinkedList);
        }

        public void Clear()
        {
            events.Clear();
        }

        // 必要に応じてその他のメソッド
    }
}