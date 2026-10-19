using MTG.Core.Abilities;
using MTG.Core.Cards;
using MTG.Core.Components;
using MTG.Core.Enums;
using MTG.Core.Helper;
using MTG.Core.OracleTextParsers;
using MTG.Core.Properties;
using System.ComponentModel;
using System.Reflection;

namespace MTG.Core.Tests.Dummy;

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

    //Enums
    public static readonly IReadOnlyDictionary<PresetCard, Func<Result<IReadOnlyList<CardFaceCreationArgs>>>> CardFaces =
        new Dictionary<PresetCard, Func<Result<IReadOnlyList<CardFaceCreationArgs>>>>
        {
            [PresetCard.Emmara] = BuildEmmaraFace,
            [PresetCard.Yshtola] = BuildYshtolaFace,
            [PresetCard.BruceBannerTheIncredibleHulk] = BuildBruceBannerAndHulkFaces,
            [PresetCard.Six] = BuildSixFace,
        };

    public static readonly IReadOnlyDictionary<PresetToken, Func<Result<IReadOnlyList<CardFaceCreationArgs>>>> TokenFaces =
        new Dictionary<PresetToken, Func<Result<IReadOnlyList<CardFaceCreationArgs>>>>
        {
            [PresetToken.SoldierTokenTGRN2] = BuildEmmaraTokenFace,
        };

    private static Result<IReadOnlyList<CardFaceCreationArgs>> BuildEmmaraFace() => BuildFace(
        name: "Emmara, Soul of the Accord",
        typeLineStr: "Legendary Creature — Elf Cleric",
        oracleText: "Whenever Emmara becomes tapped, create a 1/1 white Soldier creature token with lifelink.",
        power: "2",
        toughness: "2",
        colors: ["G", "W"]
    );

    private static Result<IReadOnlyList<CardFaceCreationArgs>> BuildEmmaraTokenFace() => BuildFace(
        name: "Soldier",
        typeLineStr: "Token Creature — Soldier",
        oracleText: "Lifelink",
        power: "1",
        toughness: "1",
        colors: ["W"]
    );

    private static Result<IReadOnlyList<CardFaceCreationArgs>> BuildYshtolaFace() => BuildFace(
        name: "Y'shtola, Night's Blessed",
        typeLineStr: "Legendary Creature — Cat Warlock",
        oracleText: "Vigilance\nAt the beginning of each end step, if a player lost 4 or more life this turn, you draw a card.\nWhenever you cast a noncreature spell with mana value 3 or greater, Y'shtola deals 2 damage to each opponent and you gain 2 life.",
        power: "2",
        toughness: "4",
        colors: ["W", "U", "B"]
    );

    private static Result<IReadOnlyList<CardFaceCreationArgs>> BuildBruceBannerFace() => BuildFace(
        name: "Bruce Banner",
        typeLineStr: "Legendary Creature — Human Scientist Hero",
        oracleText: "",
        power: "1",
        toughness: "1",
        colors: ["U"]
    );

    private static Result<IReadOnlyList<CardFaceCreationArgs>> BuildTheIncredibleHulkFace() => BuildFace(
        name: "The Incredible Hulk",
        typeLineStr: "Legendary Creature — Gamma Berserker Hero",
        oracleText: "",
        power: "8",
        toughness: "8",
        colors: ["R", "G"]
    );

    private static Result<IReadOnlyList<CardFaceCreationArgs>> BuildSixFace() => BuildFace(
        name: "Six",
        typeLineStr: "Legendary Creature — Treefolk",
        oracleText: "Reach\nWhenever Six attacks, mill three cards. You may put a land card from among them into your hand.\nDuring your turn, nonland permanent cards in your graveyard have retrace. (You may cast permanent cards from your graveyard by discarding a land card in addition to paying their other costs.)",
        power: "2",
        toughness: "4",
        colors: ["G"]
    );

    private static Result<IReadOnlyList<CardFaceCreationArgs>> BuildBruceBannerAndHulkFaces()
    {
        var frontResult = BuildBruceBannerFace();
        if (frontResult.IsFailure)
            return frontResult.ToFailure<IReadOnlyList<CardFaceCreationArgs>>();

        var backResult = BuildTheIncredibleHulkFace();
        if (backResult.IsFailure)
            return backResult.ToFailure<IReadOnlyList<CardFaceCreationArgs>>();

        IReadOnlyList<CardFaceCreationArgs> faces = [frontResult.Value[0], backResult.Value[0]];

        return Result<IReadOnlyList<CardFaceCreationArgs>>.Success(faces);
    }

    public static readonly IReadOnlyDictionary<PresetCard, Func<IReadOnlyList<ICardFace>, CardCreationArgs>> Cards =
        new Dictionary<PresetCard, Func<IReadOnlyList<ICardFace>, CardCreationArgs>>
        {
            [PresetCard.Emmara] = faces => new CardCreationArgs
            {
                ColorIdentity = ManaType.Green | ManaType.White,
                CardFaces = faces,
                AllParts = [new CardRef() { Id = new Guid("45907b16-af17-4237-ab38-9d7537fd30e8") }],
                Set = "grn",
                CollectorNumber = "168",
                Lang = "en",
                SetName = "Guilds of Ravnica",
                Id = new Guid("41b930ee-e16b-4612-87de-c03ecc6ff6db"),
                Layout = Layout.Normal,
                Rarity = Rarity.Rare,
                Legalities = new Dictionary<Format, Legality>(),
                ImageUris = new Dictionary<ImageSize, Uri>()
            },
            [PresetCard.Yshtola] = faces => new CardCreationArgs
            {
                ColorIdentity = ManaType.White | ManaType.Blue | ManaType.Black,
                CardFaces = faces,
                AllParts = [],
                Set = "fic",
                CollectorNumber = "7",
                Lang = "en",
                SetName = "Final Fantasy Commander",
                Id = new Guid("c7f2c2d5-e052-49e8-b5de-712858c2ea78"),
                Layout = Layout.Normal,
                Rarity = Rarity.Rare,
                Legalities = new Dictionary<Format, Legality>(),
                ImageUris = new Dictionary<ImageSize, Uri>()
            },
            [PresetCard.BruceBannerTheIncredibleHulk] = faces => new CardCreationArgs
            {
                ColorIdentity = ManaType.Green | ManaType.Blue | ManaType.Red,
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
            },
            [PresetCard.Six] = faces => new CardCreationArgs
            {
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
            },
        };

    public static readonly IReadOnlyDictionary<PresetToken, Func<IReadOnlyList<ICardFace>, CardCreationArgs>> Tokens =
        new Dictionary<PresetToken, Func<IReadOnlyList<ICardFace>, CardCreationArgs>>
        {
            [PresetToken.SoldierTokenTGRN2] = faces => new CardCreationArgs
            {
                ColorIdentity = ManaType.White,
                CardFaces = faces,
                AllParts = [],
                Set = "tgrn",
                CollectorNumber = "2",
                Lang = "en",
                SetName = "Guilds of Ravnica Tokens",
                Id = new Guid("45907b16-af17-4237-ab38-9d7537fd30e8"),
                Layout = Layout.Normal,
                Rarity = Rarity.Rare,
                Legalities = new Dictionary<Format, Legality>(),
                ImageUris = new Dictionary<ImageSize, Uri>()
            },
        };

    private static Result<IReadOnlyList<CardFaceCreationArgs>> BuildFace(
        string name, string typeLineStr, string oracleText, string power, string toughness,
        List<string> colors)
    {
        var typeLineResult = TypeLine.Create(typeLineStr);
        if (typeLineResult.IsFailure)
            return typeLineResult.ToFailure<IReadOnlyList<CardFaceCreationArgs>>();

        var creatureResult = CreatureComponent.Create(power, toughness);
        if (creatureResult.IsFailure)
            return creatureResult.ToFailure<IReadOnlyList<CardFaceCreationArgs>>();

        var colorResult = ColorComponent.Create(new ManaSymbolParser(), colors, null);
        if (colorResult.IsFailure)
            return colorResult.ToFailure<IReadOnlyList<CardFaceCreationArgs>>();

        var components = new List<ICardComponent> { creatureResult.Value, colorResult.Value };

        if (!string.IsNullOrWhiteSpace(oracleText))
        {
            var parseResult = new OracleTextParser().Parse(oracleText, new CardContext(name));

            if (parseResult.IsFailure)
                return parseResult.ToFailure<IReadOnlyList<CardFaceCreationArgs>>();

            components.AddRange(parseResult.Value);
        }

        IReadOnlyList<CardFaceCreationArgs> faces = [new CardFaceCreationArgs
        {
            Name = name,
            TypeLine = typeLineResult.Value,
            OracleText = oracleText,
            Components = components
        }];

        return Result<IReadOnlyList<CardFaceCreationArgs>>.Success(faces);
    }
}