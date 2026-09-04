using MTG.Core.Cards;
using MTG.Core.Helper;

namespace MTG.Core.Decks;

public abstract class Deck
{
    public IReadOnlyList<ICard> Cards { get; init; }
    public IReadOnlyList<ICard> Tokens { get; init; }

    protected Deck(IReadOnlyList<ICard> cards, IReadOnlyList<ICard> tokens)
    {
        Cards = cards;
        Tokens = tokens;
    }
}
