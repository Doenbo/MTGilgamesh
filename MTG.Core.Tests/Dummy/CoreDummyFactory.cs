using MTG.Core.Cards;
using MTG.Core.Decks;
using MTG.Core.Enums;
using MTG.Core.Helper;

namespace MTG.Core.Tests.Dummy;

public class CoreDummyFactory
{
    public static Result<ICard> CreatePresetCard(PresetCard pc)
    {
        var faceArgsResult = PresetCardData.CardFaces[pc]();
        if (faceArgsResult.IsFailure) return faceArgsResult.ToFailure<ICard>();

        var cardFaces = new List<ICardFace>();
        foreach (var args in faceArgsResult.Value)
        {
            var faceResult = CardFaceFactory.Create(args);
            if (faceResult.IsFailure) return faceResult.ToFailure<ICard>();

            cardFaces.Add(faceResult.Value);
        }

        var cardArgs = PresetCardData.Cards[pc](cardFaces);
        return CardFactory.Create(cardArgs);
    }

    public static Result<ICard> CreatePresetToken(PresetToken pt)
    {
        var faceArgsResult = PresetCardData.TokenFaces[pt]();
        if (faceArgsResult.IsFailure) return faceArgsResult.ToFailure<ICard>();

        var cardFaces = new List<ICardFace>();
        foreach (var args in faceArgsResult.Value)
        {
            var faceResult = CardFaceFactory.Create(args);
            if (faceResult.IsFailure) return faceResult.ToFailure<ICard>();

            cardFaces.Add(faceResult.Value);
        }

        var cardArgs = PresetCardData.Tokens[pt](cardFaces);
        return CardFactory.Create(cardArgs);
    }

    public static Result<ICommanderDeck> CreateCommanderDeck(List<ICard> comms)
    {
        var cards = CreateFillerCards(comms);
        var args = new CommanderDeckCreationArgs
        {
            Cards = cards.Value,
            Tokens = [],
            Commander = comms,
        };

        return CommanderDeckFactory.Create(args);
    }

    private static Result<List<ICard>> CreateFillerCards(List<ICard> comms)
    {
        var mt = comms.Aggregate(ManaType.None, (current, card) => current | card.ColorIdentity);
        var cards = new List<ICard>();
        for (int i = 0; i < 99; i++)
        {
            var icard = PresetCardData.CreateFillerCard($"Name{i}", mt);
            if (icard.IsFailure) return icard.ToFailure<List<ICard>>();
            cards.AddRange(icard.Value);
        }

        return Result<List<ICard>>.Success(cards);
    }

    private static Result<List<ICardFace>> CreateCardFace(CardFaceCreationArgs args)
    {
        var cf = CardFaceFactory.Create(args);
        if (cf.IsFailure) return cf.ToFailure<List<ICardFace>>();
        return Result<List<ICardFace>>.Success([cf.Value]);
    }

    private static Result<List<ICard>> CreateCard(CardCreationArgs args)
    {
        var c = CardFactory.Create(args);
        if (c.IsFailure) return c.ToFailure<List<ICard>>();
        return Result<List<ICard>>.Success([c.Value]);
    }
}
