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
        Assert.NotNull(act);
        Assert.Equal(new Color(), act.ColorIdentity);
        Assert.Equal(new Color(), act.ColorIndicator);
        Assert.Equal(new Color(), act.Colors);
    }

    [Theory]
    [InlineData("W", Color.White)]
    [InlineData("U", Color.Blue)]
    [InlineData("B", Color.Black)]
    [InlineData("R", Color.Red)]
    [InlineData("G", Color.Green)]
    public void CreateValidSingle(string s, Color c)
    {
        var identity = new List<string> { s };
        var colors = new List<string> { s };
        var indicator = new List<string> { s };

        var result = ColorComponent.Create(identity, colors, indicator);
        Assert.True(result.IsSuccess);

        var act = result.Value;
        Assert.NotNull(act);

        Assert.Equal(c, act.ColorIdentity);
        Assert.True(act.ColorIdentity.HasFlag(c));

        Assert.Equal(c, act.Colors);
        Assert.True(act.Colors.HasFlag(c));

        Assert.Equal(c, act.ColorIndicator);
        Assert.True(act.ColorIndicator.HasFlag(c));
    }

    [Theory]
    [InlineData("W", Color.White, "B", Color.Black)]
    public void CreateValidMultiple(string s1, Color c1, string s2, Color c2)
    {
        var identity = new List<string> { s1, s2 };
        var colors = new List<string> { s1, s2 };
        var indicator = new List<string> { s1, s2 };

        var result = ColorComponent.Create(identity, colors, indicator);
        Assert.True(result.IsSuccess);

        var act = result.Value;
        Assert.NotNull(act);

        Assert.True(act.ColorIdentity.HasFlag(c1));
        Assert.True(act.ColorIdentity.HasFlag(c2));

        Assert.True(act.Colors.HasFlag(c1));
        Assert.True(act.Colors.HasFlag(c2));

        Assert.True(act.ColorIndicator.HasFlag(c1));
        Assert.True(act.ColorIndicator.HasFlag(c2));
    }
}
