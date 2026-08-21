namespace MTG.Engine.Events;

public interface IGameEvent
{
    DateTime Timestamp { get; }
    string Description { get; }
}
