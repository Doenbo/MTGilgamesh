using MTG.Core.Enums;
using MTG.Core.Properties;

namespace MTG.Core.Tests;

public class ManaSymbolTests
{
    public static IEnumerable<object[]> ValidManaStrings =>
        [
            ["W", 1],
            ["C", 1],
            ["14", 14],
            ["W/U", 1],
            ["2/B", 2],
            ["14/B", 14],
            ["G/P", 1],
            ["X", 0]
        ];

    //Create

    [Theory]
    [MemberData(nameof(ValidManaStrings))]
    public void TestCreateValid(string input, float _)
    {
        var act = ManaSymbol.Create(input);
        Assert.True(act.IsSuccess);
        Assert.Equal($"{{{input}}}", act.Value.ToString());
    }

    [Fact]
    public void TestCreateColorless()
    {
        var input = "1";
        var act = ManaSymbol.Create(input);
        Assert.True(act.IsSuccess);
        Assert.Equal($"{{{input}}}", act.Value.ToString());
        Assert.True(act.Value.IsGenericOnly);
    }

    [Theory]
    [InlineData("")]
    [InlineData("Z")]
    [InlineData("W/Z")]
    [InlineData("-1")]
    [InlineData("-2/B")]
    [InlineData("ABC/B")]
    public void TestCreateInvalid(string input)
    {
        Assert.True(ManaSymbol.Create(input).IsFailure);
    }

    [Fact]
    public void TestCreateNull()
    {
        Assert.True(ManaSymbol.Create(null!).IsFailure);
    }

    //CMC

    [Theory]
    [MemberData(nameof(ValidManaStrings))]
    public void TestGetCMCValid(string input, float exp)
    {
        var act = ManaSymbol.Create(input);
        Assert.True(act.IsSuccess);
        var cmc = act.Value.GetCMC();
        Assert.True(cmc.IsSuccess);
        Assert.Equal(exp, cmc.Value);
    }

    [Fact]
    public void TestGetCMCInvalid()
    {
        //TODO Cannot create an invalid cmc
    }

    //Parse

    [Theory]
    [InlineData("W", ManaType.White, 0)]
    [InlineData("U", ManaType.Blue, 0)]
    [InlineData("B", ManaType.Black, 0)]
    [InlineData("R", ManaType.Red, 0)]
    [InlineData("G", ManaType.Green, 0)]
    [InlineData("14", ManaType.Colorless, 14)]
    [InlineData("W/U", ManaType.White, 0)]
    [InlineData("W/U", ManaType.Blue, 0)]
    [InlineData("2/B", ManaType.Black, 2)]
    [InlineData("X", ManaType.Colorless, 0)]
    public void TestParseValue(string input, ManaType exp_ac, int exp_gc)
    {
        var act = ManaSymbol.Create(input);
        Assert.True(act.IsSuccess);

        var ac = act.Value.AcceptedColors;
        if (exp_ac != ManaType.Colorless)
            Assert.Contains(exp_ac, ac);

        var gc = act.Value.GenericCost;
        Assert.Equal(exp_gc, gc);
    }
}
