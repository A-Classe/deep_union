using System.Collections.Generic;

namespace Core.Model.User
{
    public class EventType
    {
        protected virtual GameEventType Type() => GameEventType.Default;

        protected virtual Dictionary<string, object> Parameters() => new();

        public GameEvent Event()
        {
            GameEvent @event = new GameEvent(Type());
            @event.SetParameter(Parameters());
            return @event;
        }
    }
    public class AssignEvent: EventType
    {
        protected override GameEventType Type() => GameEventType.Assign;

        protected override Dictionary<string, object> Parameters() => new();
    }
    
    public class ReleaseEvent: EventType
    {
        protected override GameEventType Type() => GameEventType.Release;

        protected override Dictionary<string, object> Parameters() => new();
    }
    
    public class AddWorker: EventType
    {
        protected override GameEventType Type() => GameEventType.AddWorker;

        protected override Dictionary<string, object> Parameters() => new();
    }
    
    public class DelWorker: EventType
    {
        protected override GameEventType Type() => GameEventType.DelWorker;

        protected override Dictionary<string, object> Parameters() => new();
    }
    
    public class MovePlayer: EventType
    {
        protected override GameEventType Type() => GameEventType.MovePlayer;

        protected override Dictionary<string, object> Parameters() => param;
        
        private Dictionary<string, object> param = new ();
        
        public enum ParamType
        {
            Distance
        }

        public MovePlayer(float distance)
        {
            param.Add(ParamType.Distance.ToString(), distance);
        }
    }
    
    public class MoveWorkers: EventType
    {
        protected override GameEventType Type() => GameEventType.MoveWorkers;

        protected override Dictionary<string, object> Parameters() => param;
        
        private Dictionary<string, object> param = new ();
        
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
        protected override GameEventType Type() => GameEventType.GamePlay;

        protected override Dictionary<string, object> Parameters() => new();
    }

    public class GameClear : EventType
    {
        protected override GameEventType Type() => GameEventType.GameClear;

        protected override Dictionary<string, object> Parameters() => new();
    }
    
    public class GameOver : EventType
    {
        protected override GameEventType Type() => GameEventType.GameOver;

        protected override Dictionary<string, object> Parameters() => new();
    }
}