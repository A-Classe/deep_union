using System.Collections.Generic;

namespace Core.Model.User
{
    public class EventType
    {
        protected virtual GameEventType Type()
        {
            return GameEventType.Default;
        }

        protected virtual Dictionary<string, object> Parameters()
        {
            return new Dictionary<string, object>();
        }

        public GameEvent Event()
        {
            GameEvent @event = new GameEvent(Type());
            @event.SetParameter(Parameters());
            return @event;
        }
    }

    public class AssignEvent : EventType
    {
        protected override GameEventType Type()
        {
            return GameEventType.Assign;
        }

        protected override Dictionary<string, object> Parameters()
        {
            return new Dictionary<string, object>();
        }
    }

    public class ReleaseEvent : EventType
    {
        protected override GameEventType Type()
        {
            return GameEventType.Release;
        }

        protected override Dictionary<string, object> Parameters()
        {
            return new Dictionary<string, object>();
        }
    }

    public class AddWorker : EventType
    {
        protected override GameEventType Type()
        {
            return GameEventType.AddWorker;
        }

        protected override Dictionary<string, object> Parameters()
        {
            return new Dictionary<string, object>();
        }
    }

    public class DelWorker : EventType
    {
        protected override GameEventType Type()
        {
            return GameEventType.DelWorker;
        }

        protected override Dictionary<string, object> Parameters()
        {
            return new Dictionary<string, object>();
        }
    }

    public class MovePlayer : EventType
    {
        protected override GameEventType Type()
        {
            return GameEventType.MovePlayer;
        }

        protected override Dictionary<string, object> Parameters()
        {
            return param;
        }

        private readonly Dictionary<string, object> param = new();

        public enum ParamType
        {
            Distance
        }

        public MovePlayer(float distance)
        {
            param.Add(ParamType.Distance.ToString(), distance);
        }
    }

    public class MoveWorkers : EventType
    {
        protected override GameEventType Type()
        {
            return GameEventType.MoveWorkers;
        }

        protected override Dictionary<string, object> Parameters()
        {
            return param;
        }

        private readonly Dictionary<string, object> param = new();

        public enum ParamType
        {
            Distance
        }

        public MoveWorkers(float distance)
        {
            param.Add(ParamType.Distance.ToString(), distance);
        }
    }

    public class GamePlay : EventType
    {
        protected override GameEventType Type()
        {
            return GameEventType.GamePlay;
        }

        protected override Dictionary<string, object> Parameters()
        {
            return new Dictionary<string, object>();
        }
    }

    public class GameClear : EventType
    {
        protected override GameEventType Type()
        {
            return GameEventType.GameClear;
        }

        protected override Dictionary<string, object> Parameters()
        {
            return new Dictionary<string, object>();
        }
    }

    public class GameOver : EventType
    {
        protected override GameEventType Type()
        {
            return GameEventType.GameOver;
        }

        protected override Dictionary<string, object> Parameters()
        {
            return new Dictionary<string, object>();
        }
    }
}