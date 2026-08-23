using MTG.Core.Enums;
using MTG.Engine.Gameplay;

namespace MTG.Engine.Events;

public record SpellCastEvent(string PlayerName, string SpellName, ManaPool PaidMana) : IGameEvent
{
    public DateTime Timestamp { get; } = DateTime.UtcNow;
    public string Description => $"{PlayerName} casts {SpellName}.";
}

public record SpellResolvedEvent(string SpellName) : IGameEvent
{
    public DateTime Timestamp { get; } = DateTime.UtcNow;
    public string Description => $"{SpellName} resolved.";
}

public record StepTransitionEvent(TurnStep Step, string ActivePlayerName) : IGameEvent
{
    public DateTime Timestamp { get; } = DateTime.UtcNow;
    public string Description => $"\n--- [{Step}] for {ActivePlayerName} ---";
}

public record DamageDealtEvent(string Source, string Target, int Amount, bool IsCombat) : IGameEvent
{
    public DateTime Timestamp { get; } = DateTime.UtcNow;
    public string Description => $"{Source} deals {Amount} damage to {Target}.";
}

public record PlayerEliminationEvent(string PlayerName) : IGameEvent
{
    public DateTime Timestamp { get; } = DateTime.UtcNow;
    public string Description => $"{PlayerName} has been ELIMINATED! ☠";
}