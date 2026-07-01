using MTG.Core.Cards;
using MTG.Core.Helper;

namespace MTG.Core.Decks;

public abstract class Deck
{
    public List<ICard> Cards { get; init; }

    protected Deck()
    {
        Cards = [];
    }

    public Result AddCard(ICard card)
    {
        Cards.Add(card);
        return Result.Success();
    }

    public Result AddCards(List<ICard> cards)
    {
        foreach (var card in cards)
        {
            Cards.Add(card);
        }
        return Result.Success();
    }

    public void Shuffle()
    {
        Cards.Shuffle();
    }
}
