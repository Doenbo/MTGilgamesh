using MTG.Core.Enums;
using MTG.Core.Properties;

namespace MTG.Core.Tests;

public class ManaSymbolTests
{
    public static IEnumerable<object[]> ValidManaStrings =>
        new List<object[]>
        {
            new object[] { "W", 1 },
            new object[] { "14", 14 },
            new object[] { "W/U", 1 },
            new object[] { "2/B", 2 },
            new object[] { "14/B", 14 },
            new object[] { "G/P", 1 },
            new object[] { "X", 0 }
        };

    [Theory]
    [MemberData(nameof(ValidManaStrings))]
    public void TestCreateValid(string input, float _)
    {
        var act = ManaSymbol.Create(input);
        Assert.True(act.IsSuccess);
        Assert.Equal($"{{{input}}}", act.Value.ToString());
    }

    [Theory]
    [MemberData(nameof(ValidManaStrings))]
    public void TestGetCMC(string input, float exp)
    {
        var act = ManaSymbol.Create(input);
        Assert.True(act.IsSuccess);
        var cmc = act.Value.GetCMC();
        Assert.True(cmc.IsSuccess);
        Assert.Equal(exp, cmc.Value);
    }

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
}
