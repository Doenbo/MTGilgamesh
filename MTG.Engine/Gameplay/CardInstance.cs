using MTG.Core.Cards;
using MTG.Core.Components;
using MTG.Core.Enums;
using MTG.Engine.States;
using System.Numerics;

namespace MTG.Engine.Gameplay;

public class CardInstance
{
    private readonly Dictionary<Type, List<ICardState>> _states = [];
    public ICardState[] DebugStates => _states.Values.SelectMany(x => x).ToArray();

    public CardInstance(ICard card, CommanderPlayer owner)
    {
        CardData = card;
        Owner = owner;
        Controller = owner;
    }

    public ICard CardData { get; set; }

    public CommanderPlayer Owner { get; set; }
    public CommanderPlayer Controller { get; set; }

    public bool IsTapped { get; set; } = false;
    public bool HasSummoningSickness { get; set; } = true;

    public int DamageMarked { get; set; }
    public Dictionary<MarkerType, int> Counters { get; } = [];


    public void AddCounter(MarkerType type, int amount = 1)
    {
        Counters.TryGetValue(type, out int current);
        Counters[type] = current + amount;
    }

    public void AddState<TState>(TState state) where TState : class, ICardState
    {
        var type = typeof(TState);
        if (!_states.TryGetValue(type, out var list))
        {
            list = [];
            _states[type] = list;
        }
        list.Add(state);
    }

    public TState? GetState<TState>() where TState : class, ICardState
    {
        if (_states.TryGetValue(typeof(TState), out var list) && list.FirstOrDefault() is TState typedState)
        {
            return typedState;
        }

        return _states.Values
            .SelectMany(x => x)
            .OfType<TState>()
            .FirstOrDefault();
    }

    public bool HasState<TState>() where TState : class, ICardState
    {
        if (_states.TryGetValue(typeof(TState), out var list) && list.Count > 0)
        {
            return true;
        }

        return _states.Values
            .SelectMany(x => x)
            .OfType<TState>()
            .Any();
    }

    public override string ToString()
    {
        return CardData.FullName;
    }
}
