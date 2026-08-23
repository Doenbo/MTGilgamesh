using MTG.Core.Components;
using MTG.Core.Components.OracleText;
using MTG.Core.Enums;
using MTG.Core.Parser;

namespace MTG.Core.Tests;

public class OracleTextParserTests
{
    private readonly OracleTextParser _parser = new();

    // Basic Lands
    [Theory]
    [InlineData("({T}: Add {W}.)", ManaType.White)]
    [InlineData("({T}: Add {U}.)", ManaType.Blue)]
    [InlineData("({T}: Add {B}.)", ManaType.Black)]
    [InlineData("({T}: Add {R}.)", ManaType.Red)]
    [InlineData("({T}: Add {G}.)", ManaType.Green)]
    [InlineData("({T}: Add {C}.)", ManaType.Colorless)]
    public void TestParseFixedSingleValid(string input, ManaType exp)
    {
        var result = _parser.Parse(input);

        Assert.True(result.IsSuccess, result.Error);
        Assert.NotNull(result.Value);
        Assert.Single(result.Value);

        var produced = Assert.IsType<ProduceManaComponent>(result.Value[0]);
        Assert.True(produced.RequiresTap);
        Assert.Single(produced.ManaUnits);

        var manaUnit = produced.ManaUnits[0];
        Assert.True(manaUnit.IsFixed);
        Assert.Equal(exp, manaUnit.ManaFixed);
    }

    [Theory]
    [InlineData("{T}: Add {C}{C}.", ManaType.Colorless, ManaType.Colorless)] // Sol Ring
    [InlineData("{T}: Add {W}{U}.", ManaType.White, ManaType.Blue)] // Azorius Chancery
    public void TestParseFixedDoubleValid(string input, ManaType exp0, ManaType exp1)
    {
        var result = _parser.Parse(input);

        Assert.True(result.IsSuccess, result.Error);
        Assert.NotNull(result.Value);
        Assert.Single(result.Value);

        var produced = Assert.IsType<ProduceManaComponent>(result.Value[0]);
        Assert.True(produced.RequiresTap);
        Assert.Equal(2, produced.ManaUnits.Count);

        Assert.Equal(exp0, produced.ManaUnits[0].ManaFixed);
        Assert.Equal(exp1, produced.ManaUnits[1].ManaFixed);
    }

    // Dual Lands & Choices
    [Theory]
    [InlineData("({T}: Add {B} or {R}.)", ManaType.Black, ManaType.Red)]
    [InlineData("({T}: Add {B} or {G}.)", ManaType.Black, ManaType.Green)]
    [InlineData("{T}: Add {W}, {U}, or {B}.", ManaType.White, ManaType.Blue, ManaType.Black)]
    public void TestParseChoiceValid(string input, params ManaType[] expectedChoices)
    {
        var result = _parser.Parse(input);

        Assert.True(result.IsSuccess, result.Error);
        Assert.NotNull(result.Value);
        Assert.Single(result.Value);

        var produced = Assert.IsType<ProduceManaComponent>(result.Value[0]);
        Assert.True(produced.RequiresTap);
        Assert.Single(produced.ManaUnits);

        var manaUnit = produced.ManaUnits[0];
        Assert.True(manaUnit.IsChoice);
        Assert.Equal(expectedChoices.Length, manaUnit.ManaChoice.Count);

        foreach (var expected in expectedChoices)
        {
            Assert.Contains(expected, manaUnit.ManaChoice);
        }
    }

    [Theory]
    [InlineData("{T}: Add one mana of any color in your commander's color identity.", ManaDynamicType.CommanderColorIdentity)]
    [InlineData("{T}, Pay 1 life: Add one mana of any color.", ManaDynamicType.AnyColor)]
    public void TestParseDynamicValid(string input, ManaDynamicType dmt)
    {
        var result = _parser.Parse(input);

        Assert.True(result.IsSuccess, result.Error);
        Assert.NotNull(result.Value);
        Assert.Single(result.Value);

        var produced = Assert.IsType<ProduceManaComponent>(result.Value[0]);
        Assert.True(produced.RequiresTap);

        var manaUnit = produced.ManaUnits[0];
        Assert.True(manaUnit.IsDynamic);
        Assert.Equal(dmt, manaUnit.ManaDynamic);
    }

    [Theory]
    [InlineData("({T}: Add {Z}.)")]
    public void TestParse_Invalid_ReturnsFailure(string input)
    {
        var result = _parser.Parse(input);

        Assert.True(result.IsFailure);
        Assert.False(string.IsNullOrWhiteSpace(result.Error));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void TestParse_Empty_ReturnsEmptyList(string input)
    {
        var result = _parser.Parse(input);

        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value);
    }
}
