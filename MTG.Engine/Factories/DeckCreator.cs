using MTG.Core.Cards;
using MTG.Core.Decks;
using MTG.Core.Helper;
using MTG.Engine.Gameplay;
using MTG.Resources.Archidekt;
using MTG.Resources.Enums;

namespace MTG.Engine.Factories;

public class DeckCreator
{
    public async static Task<Result<CommanderDeck>> CreateCommanderPrecon(CommanderPrecon cp)
    {
        var cardrefs = ArchidektDeckImporter.ImportCommanderPrecon(cp);
        if (cardrefs.IsFailure)
            return cardrefs.ToFailure<CommanderDeck>();

        return await Create(cardrefs.Value);

    }

    public async static Task<Result<CommanderDeck>> CreateCommanderDeckFromText(string path)
    {
        var cardrefs = ArchidektDeckImporter.ImportDeckFromText(path);
        if (cardrefs.IsFailure)
            return cardrefs.ToFailure<CommanderDeck>();

        return await Create(cardrefs.Value);
    }

    private async static Task<Result<CommanderDeck>> Create(List<CardRef> cardrefs)
    {
        var deck = new CommanderDeck();

        foreach (var cardref in cardrefs)
        {
            var card = await CardCreator.GetExact(cardref);
            if (card.IsFailure)
                return card.ToFailure<CommanderDeck>();

            for (int i = 0; i < cardref.Quantity; i++)
            {
                if (cardref.Type.Contains("Commander")) //TODO?
                    deck.AddCommander(card.Value);
                else
                    deck.AddCard(card.Value);
            }
        }

        var valid = deck.IsValidCommanderDeck();
        if (valid.IsFailure)
            return valid.ToFailure<CommanderDeck>();

        return Result<CommanderDeck>.Success(deck);
    }
}