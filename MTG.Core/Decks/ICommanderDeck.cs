using MTG.Core.Cards;
using MTG.Core.Enums;
using MTG.Core.Helper;

namespace MTG.Core.Decks;

public interface ICommanderDeck
{
    IReadOnlyList<ICard> Cards { get; }

    IReadOnlyList<ICard> Tokens { get; }

    IReadOnlyList<ICard> Commander { get; }

    Result<ICard> GetRandomCard();

    Result<ManaType> GetDeckColorIdentity();

    Result<bool> IsValidCommanderDeck();
}
