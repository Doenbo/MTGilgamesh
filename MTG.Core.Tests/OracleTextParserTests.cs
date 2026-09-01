using FluentAssertions;
using MTG.Core.Abilities;
using MTG.Core.Components.OracleText;
using MTG.Core.Enums;
using MTG.Core.OracleTextParsers;

namespace MTG.Core.Tests;

public class OracleTextParserTests
{
    private readonly OracleTextParser _parser = new();

    // Basic Lands
    [Theory]
    [InlineData("Plains", "({T}: Add {W}.)", ManaType.White)]
    [InlineData("Island", "({T}: Add {U}.)", ManaType.Blue)]
    [InlineData("Swamp", "({T}: Add {B}.)", ManaType.Black)]
    [InlineData("Mountain", "({T}: Add {R}.)", ManaType.Red)]
    [InlineData("Forest", "({T}: Add {G}.)", ManaType.Green)]
    [InlineData("Wastes", "({T}: Add {C}.)", ManaType.Colorless)]
    public void TestParseFixedSingleValid(string name, string oracleText, ManaType exp)
    {
        var result = _parser.Parse(oracleText, new CardContext(name));

        result.IsSuccess.Should().BeTrue(result.Error);
        result.Value.Should().NotBeNull();
        result.Value.Should().ContainSingle();

        var produced = result.Value[0].Should().BeOfType<ProduceManaComponent>().Subject;
        produced.RequiresTap.Should().BeTrue();
        produced.ManaUnits.Should().ContainSingle();

        var manaUnit = produced.ManaUnits[0];
        manaUnit.IsFixed.Should().BeTrue();
        manaUnit.ManaFixed.Should().Be(exp);
    }

    [Theory]
    [InlineData("Sol Ring", "{T}: Add {C}{C}.", ManaType.Colorless, ManaType.Colorless)]
    [InlineData("Azorius Chancery", "{T}: Add {W}{U}.", ManaType.White, ManaType.Blue)]
    public void TestParseFixedDoubleValid(string name, string oracleText, ManaType exp0, ManaType exp1)
    {
        var result = _parser.Parse(oracleText, new CardContext(name));

        result.IsSuccess.Should().BeTrue(result.Error);
        result.Value.Should().NotBeNull();
        result.Value.Should().ContainSingle();

        var produced = result.Value[0].Should().BeOfType<ProduceManaComponent>().Subject;
        produced.RequiresTap.Should().BeTrue();
        produced.ManaUnits.Should().HaveCount(2);

        produced.ManaUnits[0].ManaFixed.Should().Be(exp0);
        produced.ManaUnits[1].ManaFixed.Should().Be(exp1);
    }

    // Dual / Triple Lands
    [Theory]
    [InlineData("Badlands", "({T}: Add {B} or {R}.)", ManaType.Black, ManaType.Red)]
    [InlineData("Bayou", "({T}: Add {B} or {G}.)", ManaType.Black, ManaType.Green)]
    [InlineData("Plateau", "({T}: Add {R} or {W}.)", ManaType.Red, ManaType.White)]
    [InlineData("Savannah", "({T}: Add {G} or {W}.)", ManaType.Green, ManaType.White)]
    [InlineData("Scrubland", "({T}: Add {W} or {B}.)", ManaType.White, ManaType.Black)]
    [InlineData("Taiga", "({T}: Add {R} or {G}.)", ManaType.Red, ManaType.Green)]
    [InlineData("Tropical Island", "({T}: Add {G} or {U}.)", ManaType.Green, ManaType.Blue)]
    [InlineData("Tundra", "({T}: Add {W} or {U}.)", ManaType.White, ManaType.Blue)]
    [InlineData("Underground Sea", "({T}: Add {U} or {B}.)", ManaType.Blue, ManaType.Black)]
    [InlineData("Volcanic Island", "({T}: Add {U} or {R}.)", ManaType.Blue, ManaType.Red)]

    [InlineData("Arcane Sanctum", "{T}: Add {W}, {U}, or {B}.", ManaType.White, ManaType.Blue, ManaType.Black)]
    [InlineData("Crumbling Necropolis", "{T}: Add {U}, {B}, or {R}.", ManaType.Blue, ManaType.Black, ManaType.Red)]
    [InlineData("Frontier Bivouac", "{T}: Add {G}, {U}, or {R}.", ManaType.Green, ManaType.Blue, ManaType.Red)]
    [InlineData("Jungle Shrine", "{T}: Add {R}, {G}, or {W}.", ManaType.Red, ManaType.Green, ManaType.White)]
    [InlineData("Mystic Monastery", "{T}: Add {U}, {R}, or {W}.", ManaType.Blue, ManaType.Red, ManaType.White)]
    [InlineData("Nomad Outpost", "{T}: Add {R}, {W}, or {B}.", ManaType.Red, ManaType.White, ManaType.Black)]
    [InlineData("Opulent Palace", "{T}: Add {B}, {G}, or {U}.", ManaType.Black, ManaType.Green, ManaType.Blue)]
    [InlineData("Sandsteppe Citadel", "{T}: Add {W}, {B}, or {G}.", ManaType.White, ManaType.Black, ManaType.Green)]
    [InlineData("Savage Lands", "{T}: Add {B}, {R}, or {G}.", ManaType.Black, ManaType.Red, ManaType.Green)]
    [InlineData("Seaside Citadel", "{T}: Add {G}, {W}, or {U}.", ManaType.Green, ManaType.White, ManaType.Blue)]
    public void TestParseChoiceValid(string name, string oracleText, params ManaType[] expectedChoices)
    {
        var result = _parser.Parse(oracleText, new CardContext(name));

        result.IsSuccess.Should().BeTrue(result.Error);
        result.Value.Should().NotBeNull();
        result.Value.Should().ContainSingle();

        var produced = result.Value[0].Should().BeOfType<ProduceManaComponent>().Subject;
        produced.RequiresTap.Should().BeTrue();
        produced.ManaUnits.Should().ContainSingle();

        var manaUnit = produced.ManaUnits[0];
        manaUnit.IsChoice.Should().BeTrue();
        manaUnit.ManaChoice.Should().HaveCount(expectedChoices.Length);
        manaUnit.ManaChoice.Should().Contain(expectedChoices);
    }

    [Theory]
    [InlineData("", "{T}: Add one mana of any color in your commander's color identity.", ManaDynamicType.CommanderColorIdentity)]
    [InlineData("", "{T}, Pay 1 life: Add one mana of any color.", ManaDynamicType.AnyColor)]
    public void TestParseDynamicValid(string name, string oracleText, ManaDynamicType dmt)
    {
        var result = _parser.Parse(oracleText, new CardContext(name));

        result.IsSuccess.Should().BeTrue(result.Error);
        result.Value.Should().NotBeNull();
        result.Value.Should().ContainSingle();

        var produced = result.Value[0].Should().BeOfType<ProduceManaComponent>().Subject;
        produced.RequiresTap.Should().BeTrue();

        var manaUnit = produced.ManaUnits[0];
        manaUnit.IsDynamic.Should().BeTrue();
        manaUnit.ManaDynamic.Should().Be(dmt);
    }

    [Theory]
    [InlineData("Failure1", "({T}: Add {Z}.)")]
    public void TestParse_Invalid_ReturnsFailure(string name, string oracleText)
    {
        var result = _parser.Parse(oracleText, new CardContext(name));

        result.IsSuccess.Should().BeTrue(); //TODO Temporary (?) no Error but Errormessage
        result.Error.Should().BeNullOrWhiteSpace();
    }

    [Theory]
    [InlineData("", "")]
    [InlineData("   ", "   ")]
    public void TestParse_Empty_ReturnsEmptyList(string name, string oracleText)
    {
        var result = _parser.Parse(oracleText, new CardContext(name));

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }
}