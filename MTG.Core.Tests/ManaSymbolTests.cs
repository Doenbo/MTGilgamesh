using MTG.Core.Properties;

namespace MTG.Core.Tests;

public class ManaSymbolTests
{
    [Theory]
    [InlineData("W")]
    [InlineData("14")]
    [InlineData("W/U")]
    [InlineData("2/B")]
    [InlineData("14/B")]
    [InlineData("G/P")]
    [InlineData("X")]
    public void CreateValid(string exp)
    {

        var act = ManaSymbol.Create(exp);
        Assert.True(act.IsSuccess);
        Assert.Equal($"{{{exp}}}", act.Value.ToString());
    }

    [Theory]
    [InlineData("")]
    [InlineData("Z")]
    [InlineData("W/Z")]
    [InlineData("-1")]
    [InlineData("-2/B")]
    [InlineData("ABC/B")]
    public void CreateInvalid(string s)
    {
        Assert.True(ManaSymbol.Create(s).IsFailure);
    }

    [Fact]
    public void CreateNull()
    {
        Assert.True(ManaSymbol.Create(null!).IsFailure);
    }
}
