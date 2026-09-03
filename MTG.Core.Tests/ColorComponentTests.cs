using FluentAssertions;
using MTG.Core.Components;
using MTG.Core.Enums;
using MTG.Core.OracleTextParsers;

namespace MTG.Core.Tests;

public class ColorComponentTests
{
    private readonly IManaSymbolParser _manaParser = new ManaSymbolParser();

    [Fact]
    public void CreateValidEmpty()
    {
        var colors = new List<string> { };
        var indicator = new List<string> { };

        var result = ColorComponent.Create(_manaParser, colors, indicator);
        result.IsSuccess.Should().BeTrue();

        var act = result.Value;
        act.Should().NotBeNull();
        act.ColorIndicator.Should().Be(new ManaType());
        act.Colors.Should().Be(new ManaType());
    }

    [Theory]
    [InlineData("W", ManaType.White)]
    [InlineData("U", ManaType.Blue)]
    [InlineData("B", ManaType.Black)]
    [InlineData("R", ManaType.Red)]
    [InlineData("G", ManaType.Green)]
    public void CreateValidSingle(string s, ManaType c)
    {
        var colors = new List<string> { s };
        var indicator = new List<string> { s };

        var result = ColorComponent.Create(_manaParser, colors, indicator);
        result.IsSuccess.Should().BeTrue();

        var act = result.Value;
        act.Should().NotBeNull();

        act.Colors.Should().Be(c);
        act.Colors.HasFlag(c).Should().BeTrue();

        act.ColorIndicator.Should().Be(c);
        act.ColorIndicator.HasFlag(c).Should().BeTrue();
    }

    [Theory]
    [InlineData("W", ManaType.White, "B", ManaType.Black)]
    public void CreateValidMultiple(string s1, ManaType c1, string s2, ManaType c2)
    {
        var colors = new List<string> { s1, s2 };
        var indicator = new List<string> { s1, s2 };

        var result = ColorComponent.Create(_manaParser, colors, indicator);
        result.IsSuccess.Should().BeTrue();

        var act = result.Value;
        act.Should().NotBeNull();

        act.Colors.HasFlag(c1).Should().BeTrue();
        act.Colors.HasFlag(c2).Should().BeTrue();
        act.ColorIndicator.HasFlag(c1).Should().BeTrue();
        act.ColorIndicator.HasFlag(c2).Should().BeTrue();
    }

    [Fact]
    public void CreateValidNulls_ReturnsManaTypeNone()
    {
        var result = ColorComponent.Create(_manaParser, null, null);

        result.IsSuccess.Should().BeTrue();
        var act = result.Value;
        act.Should().NotBeNull();
        act.Colors.Should().Be(ManaType.None);
        act.ColorIndicator.Should().Be(ManaType.None);
    }

    [Fact]
    public void CreateValidExplicitColorless_ReturnsManaTypeColorless()
    {
        var colorlessList = new List<string> { "C" };

        var result = ColorComponent.Create(_manaParser, colorlessList, colorlessList);

        result.IsSuccess.Should().BeTrue();
        var act = result.Value;
        act.Colors.Should().Be(ManaType.Colorless);
        act.ColorIndicator.Should().Be(ManaType.Colorless);
    }

    [Fact]
    public void CreateValidMixed_ParsesEachFieldIndependently()
    {
        var colors = new List<string> { "W", "U" };
        List<string>? indicator = null;

        var result = ColorComponent.Create(_manaParser, colors, indicator);

        result.IsSuccess.Should().BeTrue();
        var act = result.Value;

        act.Colors.Should().Be(ManaType.White | ManaType.Blue);
        act.ColorIndicator.Should().Be(ManaType.None);
    }

    [Theory]
    [InlineData("w", ManaType.White)]
    [InlineData("g", ManaType.Green)]
    public void CreateValidLowerCase_ParsesCorrectly(string input, ManaType expected)
    {
        var list = new List<string> { input };

        var result = ColorComponent.Create(_manaParser, list, list);

        result.IsSuccess.Should().BeTrue();
        result.Value.Colors.Should().Be(expected);
    }

    [Theory]
    [InlineData("X")]
    [InlineData("1")]
    [InlineData("")]
    [InlineData("Green")]
    public void CreateInvalidColor_ReturnsFailure(string invalidColor)
    {
        var invalidList = new List<string> { "W", invalidColor };

        var result = ColorComponent.Create(_manaParser, invalidList, null);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain($"Color '{invalidColor}' is invalid!");
    }
}
