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
        List<ICard> cards = [], tokens = [], commander = [];

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
                    commander.Add(cardResult.Value);
                else
                    cards.Add(cardResult.Value);
            }
        }

        var args = new CommanderDeckCreationArgs
        {
            Cards = cards,
            Tokens = tokens,
            Commander = commander,
        };

        //Finally Create the Commander Deck
        var deck = CommanderDeckFactory.Create(args);
        if (deck.IsFailure)
            return deck.ToFailure<ICommanderDeck>();

        //TODO maybe do this in the create?
        var validResult = deck.Value.IsValidCommanderDeck();
        if (validResult.IsFailure)
            return validResult.ToFailure<ICommanderDeck>();

        return Result<ICommanderDeck>.Success(deck.Value);
    }
}