using MTG.Core.Cards;
using MTG.Core.Decks;
using MTG.Core.Helper;
using MTG.Resources.Archidekt;
using MTG.Resources.Enums;

namespace MTG.Engine.Factories;

public static class DeckCreator
{
    public static async Task<Result<CommanderDeck>> CreateCommanderPrecon(CommanderPrecon cp)
    {
        var cardrefs = ArchidektDeckImporter.ImportCommanderPrecon(cp);
        if (cardrefs.IsFailure)
            return cardrefs.ToFailure<CommanderDeck>();

        return await Create(cardrefs.Value);
    }

    public static async Task<Result<CommanderDeck>> CreateCommanderDeckFromText(string path)
    {
        var cardrefs = ArchidektDeckImporter.ImportDeckFromText(path);
        if (cardrefs.IsFailure)
            return cardrefs.ToFailure<CommanderDeck>();

        return await Create(cardrefs.Value);
    }

    private static async Task<Result<CommanderDeck>> Create(List<CardRef> cardrefs)
    {
        var deck = new CommanderDeck();

        foreach (var cardref in cardrefs)
        {
            var cardResult = await CardCreator.GetExact(cardref);
            if (cardResult.IsFailure)
                return cardResult.ToFailure<CommanderDeck>();

            for (int i = 0; i < cardref.Quantity; i++)
            {
                // Assign to Commander zone or main deck based on Archidekt metadata
                if (cardref.Type.Contains("Commander", StringComparison.OrdinalIgnoreCase))
                {
                    var addResult = deck.AddCommander(cardResult.Value);
                    if (addResult.IsFailure)
                        return Result<CommanderDeck>.Failure(addResult.Error);
                }
                else
                {
                    deck.AddCard(cardResult.Value);
                }
            }
        }

        var validResult = deck.IsValidCommanderDeck();
        if (validResult.IsFailure)
            return validResult.ToFailure<CommanderDeck>();

        return Result<CommanderDeck>.Success(deck);
    }
}