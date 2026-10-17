using MTG.Core.Cards;
using MTG.Core.Components;
using MTG.Core.Enums;
using MTG.Core.Properties;
using System;
using System.Collections.Generic;
using System.Text;

namespace MTG.Engine.Tests.Dummy;

public static class PresetCards
{
    //public static ICard GrizzlyBears => DummyFactory.CreateCard(
    //    name: "Grizzly Bears",
    //    typeLine: "Creature — Bear"
    //);

    //public static ICard LightningBolt()
    //{
    //    //TODO result check failure?
    //    return DummyFactory.CreateCard(name: "Lightning Bolt").Value;
    //}

    public static CardCreationArgs Name(IReadOnlyList<ICardFace> cardFaces, string name) => new()
    {
        Name = name,
        TypeLine = "Legendary Creature — Treefolk", //TODO STRING
        ColorIdentity = ManaType.Green,
        CardFaces = cardFaces,
        AllParts = [],
        Set = "mh3",
        CollectorNumber = "169",
        Lang = "en",
        SetName = "Modern Horizons 3",
        Id = new Guid("f9246b68-580f-4f53-883d-7900880e4b0d"),
        Layout = Layout.Normal,
        Rarity = Rarity.Rare,
        Legalities = new Dictionary<Format, Legality>(),
        ImageUris = new Dictionary<ImageSize, Uri>(),
    };

    public static CardFaceCreationArgs NameF(string name) => new()
    {
        Name = name,
        TypeLine = TypeLine.Create("Legendary Creature — Treefolk").Value, //TODO TYPE
        OracleText = "Reach\nWhenever Six attacks, mill three cards. You may put a land card from among them into your hand.\nDuring your turn, nonland permanent cards in your graveyard have retrace. (You may cast permanent cards from your graveyard by discarding a land card in addition to paying their other costs.)",
        Components = [CreatureComponent.Create("2", "4").Value] //TODO value?
    };

    public static CardCreationArgs Six(IReadOnlyList<ICardFace> cardFaces) => new()
    {
        Name = "Six",
        TypeLine = "Legendary Creature — Treefolk", //TODO STRING
        ColorIdentity = ManaType.Green,
        CardFaces = cardFaces,
        AllParts = [],
        Set = "mh3",
        CollectorNumber = "169",
        Lang = "en",
        SetName = "Modern Horizons 3",
        Id = new Guid("f9246b68-580f-4f53-883d-7900880e4b0d"),
        Layout = Layout.Normal,
        Rarity = Rarity.Rare,
        Legalities = new Dictionary<Format, Legality>(),
        ImageUris = new Dictionary<ImageSize, Uri>(),
    };

    public static CardFaceCreationArgs SixF() => new()
    {
        Name = "Six",
        TypeLine = TypeLine.Create("Legendary Creature — Treefolk").Value, //TODO TYPE
        OracleText = "Reach\nWhenever Six attacks, mill three cards. You may put a land card from among them into your hand.\nDuring your turn, nonland permanent cards in your graveyard have retrace. (You may cast permanent cards from your graveyard by discarding a land card in addition to paying their other costs.)",
        Components = [CreatureComponent.Create("2", "4").Value] //TODO value?
    };
}