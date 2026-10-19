using MTG.Core.Cards;
using MTG.Core.Enums;
using MTG.Core.Helper;

using PCL = MTG.Core.Tests.Dummy.PresetCardLogic;

namespace MTG.Core.Tests.Dummy;

public static class PresetCardData
{
    //Enums
    public static readonly IReadOnlyDictionary<PresetCard, Func<Result<IReadOnlyList<CardFaceCreationArgs>>>> CardFaces =
        new Dictionary<PresetCard, Func<Result<IReadOnlyList<CardFaceCreationArgs>>>>
        {
            [PresetCard.Emmara] = () => PCL.SingleFace(BuildEmmaraFace()),
            [PresetCard.Yshtola] = () => PCL.SingleFace(BuildYshtolaFace()),
            [PresetCard.BruceBannerTheIncredibleHulk] = () => PCL.MultiFace(BuildBruceBannerFace(), BuildTheIncredibleHulkFace()),
            [PresetCard.Six] = () => PCL.SingleFace(BuildSixFace()),
        };

    public static readonly IReadOnlyDictionary<PresetToken, Func<Result<IReadOnlyList<CardFaceCreationArgs>>>> TokenFaces =
        new Dictionary<PresetToken, Func<Result<IReadOnlyList<CardFaceCreationArgs>>>>
        {
            [PresetToken.SoldierTokenTGRN2] = () => PCL.SingleFace(BuildEmmaraTokenFace()),
        };

    private static Result<CardFaceCreationArgs> BuildEmmaraFace() => PCL.BuildFace(
        name: "Emmara, Soul of the Accord",
        typeLineStr: "Legendary Creature — Elf Cleric",
        oracleText: "Whenever Emmara becomes tapped, create a 1/1 white Soldier creature token with lifelink.",
        power: "2",
        toughness: "2",
        colors: ["G", "W"]
    );

    private static Result<CardFaceCreationArgs> BuildEmmaraTokenFace() => PCL.BuildFace(
        name: "Soldier",
        typeLineStr: "Token Creature — Soldier",
        oracleText: "Lifelink",
        power: "1",
        toughness: "1",
        colors: ["W"]
    );

    private static Result<CardFaceCreationArgs> BuildYshtolaFace() => PCL.BuildFace(
        name: "Y'shtola, Night's Blessed",
        typeLineStr: "Legendary Creature — Cat Warlock",
        oracleText: "Vigilance\nAt the beginning of each end step, if a player lost 4 or more life this turn, you draw a card.\nWhenever you cast a noncreature spell with mana value 3 or greater, Y'shtola deals 2 damage to each opponent and you gain 2 life.",
        power: "2",
        toughness: "4",
        colors: ["W", "U", "B"]
    );

    private static Result<CardFaceCreationArgs> BuildBruceBannerFace() => PCL.BuildFace(
        name: "Bruce Banner",
        typeLineStr: "Legendary Creature — Human Scientist Hero",
        oracleText: "",
        power: "1",
        toughness: "1",
        colors: ["U"]
    );

    private static Result<CardFaceCreationArgs> BuildTheIncredibleHulkFace() => PCL.BuildFace(
        name: "The Incredible Hulk",
        typeLineStr: "Legendary Creature — Gamma Berserker Hero",
        oracleText: "",
        power: "8",
        toughness: "8",
        colors: ["R", "G"]
    );

    private static Result<CardFaceCreationArgs> BuildSixFace() => PCL.BuildFace(
        name: "Six",
        typeLineStr: "Legendary Creature — Treefolk",
        oracleText: "Reach\nWhenever Six attacks, mill three cards. You may put a land card from among them into your hand.\nDuring your turn, nonland permanent cards in your graveyard have retrace. (You may cast permanent cards from your graveyard by discarding a land card in addition to paying their other costs.)",
        power: "2",
        toughness: "4",
        colors: ["G"]
    );

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
                Id = new Guid("e0dbbdcf-84e1-494f-8b8c-0a094f603fa9"),
                Layout = Layout.ModalDfc,
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
}
