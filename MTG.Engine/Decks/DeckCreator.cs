using MTG.Core.Cards;
using MTG.Core.Decks;
using MTG.Core.Helper;
using MTG.Engine.Cards;
using MTG.Resources.Archidekt;
using MTG.Resources.Enums;

namespace MTG.Engine.Decks;

public static class DeckCreator
{
    public static async Task<Result<ICommanderDeck>> CreateCommanderPrecon(CommanderPrecon cp)
    {
        var cardrefs = ArchidektDeckImporter.ImportCommanderPrecon(cp);
        if (cardrefs.IsFailure)
            return cardrefs.ToFailure<ICommanderDeck>();

        return await Create(cardrefs.Value);
    }

    public static async Task<Result<ICommanderDeck>> CreateCommanderDeckFromText(string path)
    {
        var cardrefs = ArchidektDeckImporter.ImportDeckFromText(path);
        if (cardrefs.IsFailure)
            return cardrefs.ToFailure<ICommanderDeck>();

        return await Create(cardrefs.Value);
    }

    private static async Task<Result<ICommanderDeck>> Create(List<CardRef> cardrefs)
    {
        ICard FirstCommander = null!, SecondCommander = null!;
        List<ICard> cards = [], tokens = [];

        foreach (var cardref in cardrefs)
        {
            var cardResult = await CardCreator.GetExact(cardref);
            if (cardResult.IsFailure)
                return cardResult.ToFailure<ICommanderDeck>();

            foreach (var token in cardResult.Value.AllParts)
            {
                var tokenResult = await CardCreator.GetExact(cardref); //TODO new method GetToken?
            }

            for (int i = 0; i < cardref.Quantity; i++)
            {

                // TODO Contains??
                if (cardref.Type.Contains("Commander", StringComparison.OrdinalIgnoreCase))
                {
                    if (FirstCommander is null)
                        FirstCommander = cardResult.Value;
                    else if (SecondCommander is null)
                        SecondCommander = cardResult.Value;
                    else
                        return Result<ICommanderDeck>.Failure("Cannot have more than two Commanders!");
                }
                else
                {
                    cards.Add(cardResult.Value);
                }
            }
        }

        //Finally Create the Commander Deck
        var deck = CommanderDeckFactory.Create();
        if (deck.IsFailure)
            return deck.ToFailure<ICommanderDeck>();

        var validResult = deck.Value.IsValidCommanderDeck();
        if (validResult.IsFailure)
            return validResult.ToFailure<ICommanderDeck>();

        return Result<ICommanderDeck>.Success(deck.Value);
    }
}