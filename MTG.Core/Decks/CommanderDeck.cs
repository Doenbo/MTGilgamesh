using MTG.Core.Cards;
using MTG.Core.Components;
using MTG.Core.Enums;
using MTG.Core.Helper;

namespace MTG.Core.Decks;

public class CommanderDeck : Deck
{
    private ICard FirstCommander { get; set; } //TODO not nullable? -> warning @constructor
    private ICard? SecondCommander { get; set; }

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

    public Result<ICard> GetFirstCommander() => Result<ICard>.Success(FirstCommander);

    public Result<ICard> GetSecondCommander()
    {
        return SecondCommander == null ?
            Result<ICard>.Failure("There is no second Commander") : Result<ICard>.Success(SecondCommander);
    }

    public Result<ICard> GetRandomCard() => Result<ICard>.Success(Cards[new Random().Next(1, Cards.Count) - 1]);

    public Result<Color> GetDeckColorIdentity()
    {
        Color result = 0;
        var fc = FirstCommander.GetCardColorIdentity();
        if (fc.IsFailure)
            return fc.ToFailure<Color>();

        result = result | fc.Value;

        if (SecondCommander == null)
            return Result<Color>.Success(result);

        var sc = SecondCommander.GetCardColorIdentity();
        if (sc.IsFailure)
            return sc.ToFailure<Color>();

        result = result | sc.Value;

        return Result<Color>.Success(result);
    }

    public Result<bool> IsValidCommanderDeck()
    {
        if (FirstCommander == null)
            return Result<bool>.Failure("No Commander!");

        if (SecondCommander == null && Cards.Count != 99)
            return Result<bool>.Failure($"Not the right amount of Cards in the Deck! {Cards.Count}/99");

        if (SecondCommander != null && Cards.Count != 98)
            return Result<bool>.Failure($"Not the right amount of Cards in the Deck! {Cards.Count}/98");

        var deckColor = GetDeckColorIdentity();
        if (deckColor.IsFailure)
            return deckColor.ToFailure<bool>();

        foreach (var card in Cards)
        {
            if (card.Legalities[Format.Commander] != Legality.Legal)
                return Result<bool>.Failure("Card is not legal in Commander!");

            foreach (var face in card.Faces)
            {
                if (!face.TryGetComponent<ColorComponent>(out var ident))
                    return Result<bool>.Failure("No Color Component?");

                var isLegal = (ident.ColorIdentity & ~deckColor.Value) == Color.Colorless;
                if (!isLegal)
                    return Result<bool>.Failure($"Illegal Card: {card}");
            }
        }

        return Result<bool>.Success(true);
    }
}
