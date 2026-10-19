using MTG.Core.Abilities;
using MTG.Core.Cards;
using MTG.Core.Components;
using MTG.Core.Enums;
using MTG.Core.Helper;
using MTG.Core.OracleTextParsers;
using MTG.Core.Properties;

namespace MTG.Core.Tests.Dummy;

public static class PresetCardLogic
{
    //Filler
    public static Result<ICard> CreateFillerCard(string cardName, ManaType mt)
    {
        var faceArgs = new CardFaceCreationArgs
        {
            Name = cardName,
            TypeLine = TypeLine.Create("Creature — Bear").Value,
            OracleText = string.Empty,
            Components = [CreatureComponent.Create("2", "2").Value]
        };

        var face = CardFaceFactory.Create(faceArgs);
        if (face.IsFailure) return face.ToFailure<ICard>();

        var cardArgs = new CardCreationArgs
        {
            ColorIdentity = mt,
            CardFaces = [face.Value],
            AllParts = [],
            Set = "tst",
            CollectorNumber = "1",
            Lang = "en",
            SetName = "Test Set",
            Id = Guid.NewGuid(),
            Layout = Layout.Normal,
            Rarity = Rarity.Common,
            Legalities = new Dictionary<Format, Legality>(),
            ImageUris = new Dictionary<ImageSize, Uri>()
        };

        return CardFactory.Create(cardArgs);
    }

    public static Result<IReadOnlyList<CardFaceCreationArgs>> SingleFace(Result<CardFaceCreationArgs> faceResult)
    {
        if (faceResult.IsFailure)
            return faceResult.ToFailure<IReadOnlyList<CardFaceCreationArgs>>();

        return Result<IReadOnlyList<CardFaceCreationArgs>>.Success([faceResult.Value]);
    }

    public static Result<IReadOnlyList<CardFaceCreationArgs>> MultiFace(params Result<CardFaceCreationArgs>[] faceResults)
    {
        var list = new List<CardFaceCreationArgs>();
        foreach (var res in faceResults)
        {
            if (res.IsFailure)
                return res.ToFailure<IReadOnlyList<CardFaceCreationArgs>>();

            list.Add(res.Value);
        }

        return Result<IReadOnlyList<CardFaceCreationArgs>>.Success(list);
    }

    public static Result<CardFaceCreationArgs> BuildFace(
        string name, string typeLineStr, string oracleText, string power, string toughness,
        List<string> colors)
    {
        var typeLineResult = TypeLine.Create(typeLineStr);
        if (typeLineResult.IsFailure)
            return typeLineResult.ToFailure<CardFaceCreationArgs>();

        var creatureResult = CreatureComponent.Create(power, toughness);
        if (creatureResult.IsFailure)
            return creatureResult.ToFailure<CardFaceCreationArgs>();

        var colorResult = ColorComponent.Create(new ManaSymbolParser(), colors, null);
        if (colorResult.IsFailure)
            return colorResult.ToFailure<CardFaceCreationArgs>();

        var components = new List<ICardComponent> { creatureResult.Value, colorResult.Value };

        if (!string.IsNullOrWhiteSpace(oracleText))
        {
            var parseResult = new OracleTextParser().Parse(oracleText, new CardContext(name));
            if (parseResult.IsFailure)
                return parseResult.ToFailure<CardFaceCreationArgs>();

            components.AddRange(parseResult.Value);
        }

        return Result<CardFaceCreationArgs>.Success(new CardFaceCreationArgs
        {
            Name = name,
            TypeLine = typeLineResult.Value,
            OracleText = oracleText,
            Components = components
        });
    }
}