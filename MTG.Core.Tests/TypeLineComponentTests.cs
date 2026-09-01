using FluentAssertions;
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
        result.IsSuccess.Should().BeTrue();
        var act = result.Value;

        act.CardTypes.Count.Should().Be(cardTypes.Length);
        act.SuperTypes.Count.Should().Be(superTypes.Length);
        act.ArtifactTypes.Count.Should().Be(artifactTypes.Length);
        act.CreatureTypes.Count.Should().Be(creatureTypes.Length);
        act.EnchantmentTypes.Count.Should().Be(enchantmentTypes.Length);
        act.LandTypes.Count.Should().Be(landTypes.Length);
        act.PlaneswalkerTypes.Count.Should().Be(planeswalkerTypes.Length);
        act.SpellTypes.Count.Should().Be(spellTypes.Length);

        act.CardTypes.Should().BeEquivalentTo(cardTypes);
        act.SuperTypes.Should().BeEquivalentTo(superTypes);
        act.ArtifactTypes.Should().BeEquivalentTo(artifactTypes);
        act.CreatureTypes.Should().BeEquivalentTo(creatureTypes);
        act.EnchantmentTypes.Should().BeEquivalentTo(enchantmentTypes);
        act.LandTypes.Should().BeEquivalentTo(landTypes);
        act.PlaneswalkerTypes.Should().BeEquivalentTo(planeswalkerTypes);
        act.SpellTypes.Should().BeEquivalentTo(spellTypes);
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

        result.IsFailure.Should().BeTrue();
        result.Error.Should().NotBeNull();
        result.Error.Should().NotBeEmpty();
    }
}
