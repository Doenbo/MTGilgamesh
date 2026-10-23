using MTG.Core.Enums;
using MTG.Core.Helper;
using MTG.Engine.Cards;
using MTG.Engine.Gameplay;

namespace MTG.Engine.Zones;

public abstract class BaseZone : IGameZone
{
    protected readonly List<CardInstance> _cards = [];

    public abstract ZoneType Type { get; }
    public abstract bool IsPublic { get; }

    public int Count => _cards.Count;
    public IReadOnlyList<CardInstance> Cards => _cards.AsReadOnly();

    //Methods
    public virtual Result AddCard(CardInstance card)
    {
        _cards.Add(card);
        return Result.Success();
    }

    public virtual Result<bool> RemoveCard(CardInstance card)
    {
        return Result<bool>.Success(_cards.Remove(card));
    }

    public virtual Result<int> RemoveAllCards(CommanderPlayer player)
    {
        return Result<int>.Success(_cards.RemoveAll(card => card.Owner == player));
    }

    public Result<bool> ContainsCard(CardInstance card)
    {
        return Result<bool>.Success(_cards.Contains(card));
    }
}