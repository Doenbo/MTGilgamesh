using FluentAssertions;
using MTG.Core.Components;
using MTG.Core.Enums;

namespace MTG.Core.Tests;

public class ColorComponentTests
{

    [Fact]
    public void CreateValidEmpty()
    {
        var identity = new List<string> { };
        var colors = new List<string> { };
        var indicator = new List<string> { };

        var result = ColorComponent.Create(colors, identity, indicator);
        result.IsSuccess.Should().BeTrue();

        var act = result.Value;
        act.Should().NotBeNull();
        act.ColorIdentity.Should().Be(new ManaType());
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
        var identity = new List<string> { s };
        var colors = new List<string> { s };
        var indicator = new List<string> { s };

        var result = ColorComponent.Create(colors, identity, indicator);
        result.IsSuccess.Should().BeTrue();

        var act = result.Value;
        act.Should().NotBeNull();

        act.ColorIdentity.Should().Be(c);
        act.ColorIdentity.HasFlag(c).Should().BeTrue();

        act.Colors.Should().Be(c);
        act.Colors.HasFlag(c).Should().BeTrue();

        act.ColorIndicator.Should().Be(c);
        act.ColorIndicator.HasFlag(c).Should().BeTrue();
    }

    [Theory]
    [InlineData("W", ManaType.White, "B", ManaType.Black)]
    public void CreateValidMultiple(string s1, ManaType c1, string s2, ManaType c2)
    {
        var identity = new List<string> { s1, s2 };
        var colors = new List<string> { s1, s2 };
        var indicator = new List<string> { s1, s2 };

        var result = ColorComponent.Create(colors, identity, indicator);
        result.IsSuccess.Should().BeTrue();

        var act = result.Value;
        act.Should().NotBeNull();

        act.ColorIdentity.HasFlag(c1).Should().BeTrue();
        act.ColorIdentity.HasFlag(c2).Should().BeTrue();
        act.Colors.HasFlag(c1).Should().BeTrue();
        act.Colors.HasFlag(c2).Should().BeTrue();
        act.ColorIndicator.HasFlag(c1).Should().BeTrue();
        act.ColorIndicator.HasFlag(c2).Should().BeTrue();
    }

    [Fact]
    public void CreateValidNulls_ReturnsManaTypeNone()
    {
        var result = ColorComponent.Create(null, null, null);

        result.IsSuccess.Should().BeTrue();
        var act = result.Value;
        act.Should().NotBeNull();
        act.ColorIdentity.Should().Be(ManaType.None);
        act.Colors.Should().Be(ManaType.None);
        act.ColorIndicator.Should().Be(ManaType.None);
    }

    [Fact]
    public void CreateValidExplicitColorless_ReturnsManaTypeColorless()
    {
        var colorlessList = new List<string> { "C" };

        var result = ColorComponent.Create(colorlessList, colorlessList, colorlessList);

        result.IsSuccess.Should().BeTrue();
        var act = result.Value;
        act.ColorIdentity.Should().Be(ManaType.Colorless);
        act.Colors.Should().Be(ManaType.Colorless);
        act.ColorIndicator.Should().Be(ManaType.Colorless);
    }

    [Fact]
    public void CreateValidMixed_ParsesEachFieldIndependently()
    {
        var identity = new List<string> { "W", "U", "B" };
        var colors = new List<string> { "W", "U" };
        List<string>? indicator = null;

        var result = ColorComponent.Create(colors, identity, indicator);

        result.IsSuccess.Should().BeTrue();
        var act = result.Value;

        act.ColorIdentity.Should().Be(ManaType.White | ManaType.Blue | ManaType.Black);
        act.Colors.Should().Be(ManaType.White | ManaType.Blue);
        act.ColorIndicator.Should().Be(ManaType.None);
    }

    [Theory]
    [InlineData("w", ManaType.White)]
    [InlineData("g", ManaType.Green)]
    public void CreateValidLowerCase_ParsesCorrectly(string input, ManaType expected)
    {
        var list = new List<string> { input };

        var result = ColorComponent.Create(list, list, list);

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

        var result = ColorComponent.Create(invalidList, null, null);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain($"Color '{invalidColor}' is invalid!");
    }
}
