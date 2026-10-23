using MTG.Core;
using MTG.Core.Cards;
using MTG.Core.Components;
using MTG.Core.Enums;
using MTG.Core.Helper;
using MTG.Engine.Gameplay;
using MTG.Engine.States;

namespace MTG.Engine.Cards;

public class CardInstance 
{
    private readonly List<ICardState> _states = [];

    public ICard CardData { get; init; }

    public CommanderPlayer Owner { get; set; }
    public CommanderPlayer Controller { get; set; }

    public bool IsTapped { get; set; } = false;
    public bool HasSummoningSickness { get; set; } = true;

    public int DamageMarked { get; set; }
    public Dictionary<MarkerType, int> Counters { get; set; } = [];

    //Factory Pattern
    private CardInstance(ICard card, CommanderPlayer owner)
    {
        CardData = card;
        Owner = owner;
        Controller = owner;
    }

    public static Result<CardInstance> Create(ICard card, CommanderPlayer owner)
    {
        var ci = new CardInstance(card, owner);
        ci.InitializeStatesFromTemplate(card.MainFace);
        return Result<CardInstance>.Success(ci);
    }

    public static Result<CardInstance> CreateToken(ICard card, CommanderPlayer owner)
    {
        throw new NotImplementedException();
    }

    public static Result<CardInstance> CreateCopy(ICard card, CommanderPlayer owner)
    {
        throw new NotImplementedException();
    }

    private void InitializeStatesFromTemplate(ICardFace face)
    {
        if (face.TryGetComponent<CreatureComponent>(out var creatureComp))
        {
            _states.Add(new CreatureState(creatureComp));
        }
    }

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
    public override string ToString() => CardData.MainFaceName;
}
