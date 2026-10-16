using MTG.Core.Cards;
using MTG.Core.Enums;
using MTG.Engine.Gameplay;
using MTG.Engine.States;

namespace MTG.Engine.Cards;

public class CardInstance
{
    private readonly List<ICardState> _states = [];

    public CardInstance(ICard card, CommanderPlayer owner)
    {
        CardData = card;
        Owner = owner;
        Controller = owner;
    }

    public ICard CardData { get; init; }

    public CommanderPlayer Owner { get; set; }
    public CommanderPlayer Controller { get; set; }

    public bool IsTapped { get; set; } = false;
    public bool HasSummoningSickness { get; set; } = true;

    public int DamageMarked { get; set; }
    public Dictionary<MarkerType, int> Counters { get; set; } = [];


    public void AddCounter(MarkerType type, int amount = 1)
    {
        Counters.TryGetValue(type, out int current);
        Counters[type] = current + amount;
    }

    public void AddState<TState>(TState state) where TState : class, ICardState
    {
        _states.Add(state);
    }

    public TState? GetState<TState>() where TState : class, ICardState
    {
        return _states.OfType<TState>().FirstOrDefault();
    }

    public IEnumerable<TState> GetStates<TState>() where TState : class, ICardState
    {
        return _states.OfType<TState>();
    }

    public bool HasState<TState>() where TState : class, ICardState
    {
        return _states.OfType<TState>().Any();
    }

    public bool RemoveState<TState>(TState state) where TState : class, ICardState
    {
        return _states.Remove(state);
    }

    public int RemoveAllStates<TState>() where TState : class, ICardState
    {
        return _states.RemoveAll(x => x is TState);
    }

    //ToStrings
    public override string ToString()
    {
        return CardData.Name;
    }
}
