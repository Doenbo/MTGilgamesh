using MTG.Core.Cards;
using MTG.Core.Components;
using MTG.Core.Enums;
using MTG.Core.Helper;
using MTG.Core.Properties;

namespace MTG.Engine.Tests.Dummy;

public static class PresetCardData
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

        ICardFace face = CardFaceFactory.Create(faceArgs).Value; //TODO is this cool?

        var cardArgs = new CardCreationArgs
        {
            Name = cardName,
            TypeLine = "Creature — Bear",
            ColorIdentity = mt,
            CardFaces = [face],
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

    //Enum
    public static readonly IReadOnlyDictionary<PresetCard, Func<Result<IReadOnlyList<CardFaceCreationArgs>>>> Faces =
        new Dictionary<PresetCard, Func<Result<IReadOnlyList<CardFaceCreationArgs>>>>
        {
            [PresetCard.Six] = () => BuildFace(
                name: "Six",
                typeLineStr: "Legendary Creature — Treefolk",
                oracleText: "Reach\nWhenever Six attacks, mill three cards. You may put a land card from among them into your hand.\nDuring your turn, nonland permanent cards in your graveyard have retrace. (You may cast permanent cards from your graveyard by discarding a land card in addition to paying their other costs.)",
                power: "2",
                toughness: "4"
            ),
        };

    public static readonly IReadOnlyDictionary<PresetCard, Func<IReadOnlyList<ICardFace>, CardCreationArgs>> Cards =
        new Dictionary<PresetCard, Func<IReadOnlyList<ICardFace>, CardCreationArgs>>
        {
            [PresetCard.Six] = faces => new CardCreationArgs
            {
                Name = "Six",
                TypeLine = "Legendary Creature — Treefolk",
                ColorIdentity = ManaType.Green,
                CardFaces = faces,
                AllParts = [],
                Set = "mh3",
                CollectorNumber = "169",
                Lang = "en",
                SetName = "Modern Horizons 3",
                Id = new Guid("f9246b68-580f-4f53-883d-7900880e4b0d"),
                Layout = Layout.Normal,
                Rarity = Rarity.Rare,
                Legalities = new Dictionary<Format, Legality>(),
                ImageUris = new Dictionary<ImageSize, Uri>()
            }
        };

    private static Result<IReadOnlyList<CardFaceCreationArgs>> BuildFace(
        string name, string typeLineStr, string oracleText, string power, string toughness)
    {
        var typeLineResult = TypeLine.Create(typeLineStr);
        var creatureResult = CreatureComponent.Create(power, toughness);

        if (typeLineResult.IsFailure)
            return typeLineResult.ToFailure<IReadOnlyList<CardFaceCreationArgs>>();

        if (creatureResult.IsFailure)
            return creatureResult.ToFailure<IReadOnlyList<CardFaceCreationArgs>>();

        IReadOnlyList<CardFaceCreationArgs> faces = [new CardFaceCreationArgs{
            Name = name,
            TypeLine = typeLineResult.Value,
            OracleText = oracleText,
            Components = [creatureResult.Value]
        }];

        return Result<IReadOnlyList<CardFaceCreationArgs>>.Success(faces);
    }
}