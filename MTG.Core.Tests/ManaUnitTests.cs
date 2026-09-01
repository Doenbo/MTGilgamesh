using FluentAssertions;
using MTG.Core.Abilities;
using MTG.Core.Enums;

namespace MTG.Core.Tests;

public class ManaUnitTests
{
    //Basic Lands
    [Theory]
    [InlineData(ManaType.White)]
    [InlineData(ManaType.Blue)]
    [InlineData(ManaType.Black)]
    [InlineData(ManaType.Red)]
    [InlineData(ManaType.Green)]
    public void TestCreateFixedValid(ManaType input)
    {
        var pmc = ManaUnit.CreateFixed(input);
        pmc.IsSuccess.Should().BeTrue();

        var produced = pmc.Value;
        produced.IsFixed.Should().BeTrue();
        produced.IsChoice.Should().BeFalse();
        produced.IsDynamic.Should().BeFalse();

        produced.ManaFixed.Should().Be(input);
        produced.ManaRestriction.Should().Be(ManaRestriction.None);
    }

    public static IEnumerable<object[]> ValidManaStrings =>
    new List<object[]>
    {
        new object[] { new List<ManaType> { ManaType.Black, ManaType.Red } }, //Badlands
        new object[] { new List<ManaType> { ManaType.White, ManaType.Blue, ManaType.Black } }, //Arcane Sanctum
    };

    [Theory]
    [MemberData(nameof(ValidManaStrings))]
    public void TestCreateChoiseValid(IReadOnlyList<ManaType> input)
    {
        var pmc = ManaUnit.CreateChoice(input);
        pmc.IsSuccess.Should().BeTrue();

        var produced = pmc.Value;
        produced.IsFixed.Should().BeFalse();
        produced.IsChoice.Should().BeTrue();
        produced.IsDynamic.Should().BeFalse();

        produced.ManaChoice.Count.Should().Be(input.Count);
        produced.ManaRestriction.Should().Be(ManaRestriction.None);
    }

    [Theory]
    [InlineData(ManaDynamicType.CommanderColorIdentity)] //Command Tower
    [InlineData(ManaDynamicType.AnyColor)] //City of Brass
    [InlineData(ManaDynamicType.OpponentLandColor)] //Exotic Orchard
    public void TestCreateDynamicValid(ManaDynamicType input)
    {
        var pmc = ManaUnit.CreateDynamic(input);
        pmc.IsSuccess.Should().BeTrue();

        var produced = pmc.Value;
        produced.IsFixed.Should().BeFalse();
        produced.IsChoice.Should().BeFalse();
        produced.IsDynamic.Should().BeTrue();

        produced.ManaDynamic.Should().Be(input);
    }
}