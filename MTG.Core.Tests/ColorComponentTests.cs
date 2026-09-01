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

        var result = ColorComponent.Create(identity, colors, indicator);
        Assert.True(result.IsSuccess);

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

        var result = ColorComponent.Create(identity, colors, indicator);
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

        var result = ColorComponent.Create(identity, colors, indicator);
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
}
