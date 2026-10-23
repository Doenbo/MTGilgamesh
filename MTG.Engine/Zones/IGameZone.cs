using MTG.Core.Enums;
using MTG.Core.Helper;
using MTG.Engine.Cards;
using MTG.Engine.Gameplay;

namespace MTG.Engine.Zones;

public interface IGameZone
{
    ZoneType Type { get; }
    bool IsPublic { get; }

    IReadOnlyList<CardInstance> Cards { get; }
    int Count { get; }

    Result AddCard(CardInstance item);
    Result<bool> RemoveCard(CardInstance item);
    Result<int> RemoveAllCards(CommanderPlayer player);
    Result<bool> ContainsCard(CardInstance item);
}