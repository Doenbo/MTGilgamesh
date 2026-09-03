using MTG.Core.Cards;
using MTG.Core.Components;
using MTG.Core.Enums;
using MTG.Core.Helper;

namespace MTG.Core.Decks;

public class CommanderDeck : Deck
{
    // Make explicit nullable to remove constructor compiler warnings
    private ICard? FirstCommander { get; set; }
    private ICard? SecondCommander { get; set; }

    // Standard constructor for builders/creators
    public CommanderDeck()
    {
    }

    public Result AddCommander(ICard card)
    {
        if (FirstCommander == null)
        {
            FirstCommander = card;
            return Result.Success();
        }

        if (SecondCommander == null)
        {
            SecondCommander = card;
            return Result.Success();
        }

        return Result.Failure("There can't be more than 2 Commanders!");
    }

    public Result<ICard> GetFirstCommander()
    {
        return FirstCommander == null
            ? Result<ICard>.Failure("No primary Commander set.")
            : Result<ICard>.Success(FirstCommander);
    }

    public Result<ICard> GetSecondCommander()
    {
        return SecondCommander == null
            ? Result<ICard>.Failure("There is no second Commander.")
            : Result<ICard>.Success(SecondCommander);
    }

    public Result<ICard> GetRandomCard()
    {
        if (Cards.Count == 0)
            return Result<ICard>.Failure("Deck is empty.");

        return Result<ICard>.Success(Cards[Random.Shared.Next(0, Cards.Count)]);
    }

    public Result<ManaType> GetDeckColorIdentity()
    {
        if (FirstCommander == null)
            return Result<ManaType>.Failure("Cannot calculate color identity without a Commander.");

        ManaType result = FirstCommander.ColorIdentity;

        if (SecondCommander == null)
            return Result<ManaType>.Success(result);

        result |= SecondCommander.ColorIdentity;

        return Result<ManaType>.Success(result);
    }

    public Result<bool> IsValidCommanderDeck()
    {
        if (FirstCommander == null)
            return Result<bool>.Failure("No Commander found in the deck!");

        int expectedCards = SecondCommander == null ? 99 : 98;
        if (Cards.Count != expectedCards)
            return Result<bool>.Failure($"Not the right amount of Cards in the Deck! {Cards.Count}/{expectedCards}");

        var deckColor = GetDeckColorIdentity();
        if (deckColor.IsFailure)
            return deckColor.ToFailure<bool>();

        foreach (var card in Cards)
        {
            if (card.Legalities.TryGetValue(Format.Commander, out var legality) && legality != Legality.Legal)
                return Result<bool>.Failure($"Card '{card}' is not legal in Commander!");

            var isLegal = (card.ColorIdentity & ~deckColor.Value) == ManaType.None;
            if (!isLegal)
                return Result<bool>.Failure($"Illegal Card for color identity: {card}");
        }

        return Result<bool>.Success(true);
    }
}