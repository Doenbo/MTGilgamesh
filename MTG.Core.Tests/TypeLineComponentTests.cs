using MTG.Core.Properties;
using MTG.Core.Types;

namespace MTG.Core.Tests;

public class TypeLineComponentTests
{
    public static IEnumerable<object[]> TypeLineValidTestData()
    {
        yield return new object[]
        {
            "Legendary Creature — Elf Cleric",
            new[] { CardType.Creature },
            new[] { SuperType.Legendary },
            Array.Empty<ArtifactType>(),
            new[] { CreatureType.Elf, CreatureType.Cleric },
            Array.Empty<EnchantmentType>(),
            Array.Empty<LandType>(),
            Array.Empty<PlaneswalkerType>(),
            Array.Empty<SpellType>(),
        };

        yield return new object[]
        {
            "Artifact — Vehicle",
            new[] { CardType.Artifact },
            Array.Empty<SuperType>(),
            new[] { ArtifactType.Vehicle },
            Array.Empty<CreatureType>(),
            Array.Empty<EnchantmentType>(),
            Array.Empty<LandType>(),
            Array.Empty<PlaneswalkerType>(),
            Array.Empty<SpellType>(),
        };

        yield return new object[]
        {
            "Basic Snow Land — Mountain",
            new[] { CardType.Land },
            new[] { SuperType.Basic, SuperType.Snow },
            Array.Empty<ArtifactType>(),
            Array.Empty<CreatureType>(),
            Array.Empty<EnchantmentType>(),
            new[] { LandType.Mountain },
            Array.Empty<PlaneswalkerType>(),
            Array.Empty<SpellType>(),
        };

        yield return new object[]
        {
            "Instant",
            new[] { CardType.Instant },
            Array.Empty<SuperType>(),
            Array.Empty<ArtifactType>(),
            Array.Empty<CreatureType>(),
            Array.Empty<EnchantmentType>(),
            Array.Empty<LandType>(),
            Array.Empty<PlaneswalkerType>(),
            Array.Empty<SpellType>(),
        };

        yield return new object[]
        {
            "Legendary Land",
            new[] { CardType.Land },
            new[] { SuperType.Legendary },
            Array.Empty<ArtifactType>(),
            Array.Empty<CreatureType>(),
            Array.Empty<EnchantmentType>(),
            Array.Empty<LandType>(),
            Array.Empty<PlaneswalkerType>(),
            Array.Empty<SpellType>(),
        };

        yield return new object[]
        {
            "Enchantment Land Artifact — Saga",
            new[] { CardType.Artifact, CardType.Enchantment, CardType.Land },
            Array.Empty<SuperType>(),
            Array.Empty<ArtifactType>(),
            Array.Empty<CreatureType>(),
            new[] { EnchantmentType.Saga },
            Array.Empty<LandType>(),
            Array.Empty<PlaneswalkerType>(),
            Array.Empty<SpellType>(),
        };

        yield return new object[]
        {
            "Legendary Planeswalker — Elspeth",
            new[] { CardType.Planeswalker },
            new[] { SuperType.Legendary },
            Array.Empty<ArtifactType>(),
            Array.Empty<CreatureType>(),
            Array.Empty<EnchantmentType>(),
            Array.Empty<LandType>(),
            new[] { PlaneswalkerType.Elspeth },
            Array.Empty<SpellType>(),
        };

        //Special case: Kindred Spells
        yield return new object[]
        {
            "Kindred Instant — Goblin",
            new[] { CardType.Instant, CardType.Kindred },
            Array.Empty<SuperType>(),
            Array.Empty<ArtifactType>(),
            new[] { CreatureType.Goblin },
            Array.Empty<EnchantmentType>(),
            Array.Empty<LandType>(),
            Array.Empty<PlaneswalkerType>(),
            Array.Empty<SpellType>(),
        };

        //Special case: Urza's Tower
        yield return new object[]
        {
            "Land — Urza's Tower",
            new[] { CardType.Land },
            Array.Empty<SuperType>(),
            Array.Empty<ArtifactType>(),
            Array.Empty<CreatureType>(),
            Array.Empty<EnchantmentType>(),
            new[] { LandType.Urzas, LandType.Tower },
            Array.Empty<PlaneswalkerType>(),
            Array.Empty<SpellType>(),
        };

        //Special case: The 'space' massacre
        yield return new object[]
        {
            "   Legendary   Creature   —   Elf   Cleric  ",
            new[] { CardType.Creature },
            new[] { SuperType.Legendary },
            Array.Empty<ArtifactType>(),
            new[] { CreatureType.Elf, CreatureType.Cleric },
            Array.Empty<EnchantmentType>(),
            Array.Empty<LandType>(),
            Array.Empty<PlaneswalkerType>(),
            Array.Empty<SpellType>(),
        };

        //Special case: Two Faced
        yield return new object[]
        {
            "Legendary Creature — Human Detective // Instant — Adventure",
            new[] { CardType.Creature, CardType.Instant },
            new[] { SuperType.Legendary },
            Array.Empty<ArtifactType>(),
            new[] { CreatureType.Human, CreatureType.Detective },
            Array.Empty<EnchantmentType>(),
            Array.Empty<LandType>(),
            Array.Empty<PlaneswalkerType>(),
            new[] { SpellType.Adventure },
        };

    }

    [Theory]
    [MemberData(nameof(TypeLineValidTestData))]
    public void CreateValid(string typeline,
        CardType[] cardTypes,
        SuperType[] superTypes,
        ArtifactType[] artifactTypes,
        CreatureType[] creatureTypes,
        EnchantmentType[] enchantmentTypes,
        LandType[] landTypes,
        PlaneswalkerType[] planeswalkerTypes,
        SpellType[] spellTypes
    )
    {

        var result = TypeLine.Create(typeline);
        Assert.True(result.IsSuccess);
        var act = result.Value;

        Assert.Equal(cardTypes.Length, act.CardTypes.Count);
        Assert.Equal(superTypes.Length, act.SuperTypes.Count);
        Assert.Equal(artifactTypes.Length, act.ArtifactTypes.Count);
        Assert.Equal(creatureTypes.Length, act.CreatureTypes.Count);
        Assert.Equal(enchantmentTypes.Length, act.EnchantmentTypes.Count);
        Assert.Equal(landTypes.Length, act.LandTypes.Count);
        Assert.Equal(planeswalkerTypes.Length, act.PlaneswalkerTypes.Count);
        Assert.Equal(spellTypes.Length, act.SpellTypes.Count);

        foreach (var _ in cardTypes)
            Assert.Contains(_, act.CardTypes);

        foreach (var _ in superTypes)
            Assert.Contains(_, act.SuperTypes);

        foreach (var _ in artifactTypes)
            Assert.Contains(_, act.ArtifactTypes);

        foreach (var _ in creatureTypes)
            Assert.Contains(_, act.CreatureTypes);

        foreach (var _ in enchantmentTypes)
            Assert.Contains(_, act.EnchantmentTypes);

        foreach (var _ in landTypes)
            Assert.Contains(_, act.LandTypes);

        foreach (var _ in planeswalkerTypes)
            Assert.Contains(_, act.PlaneswalkerTypes);

        foreach (var _ in spellTypes)
            Assert.Contains(_, act.SpellTypes);
    }

    public static IEnumerable<object[]> TypeLineInvalidTestData()
    {
        yield return new object[] { "Cookie" };
        yield return new object[] { "Legendary Creature — Vegan Elf" };
        yield return new object[] { "— Goblin" };
        yield return new object[] { "Legendary Snow" };
        yield return new object[] { "   " };
    }
    [Theory]
    [MemberData(nameof(TypeLineInvalidTestData))]
    public void CreateInvalid(string invalidTypeLine)
    {
        var result = TypeLine.Create(invalidTypeLine);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
        Assert.NotEmpty(result.Error);
    }
}
