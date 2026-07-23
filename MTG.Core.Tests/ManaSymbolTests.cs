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
    public void TestCreateValid(string exp, float _)
    {

        var act = ManaSymbol.Create(exp);
        Assert.True(act.IsSuccess);
        Assert.Equal($"{{{exp}}}", act.Value.ToString());
    }

    [Theory]
    [MemberData(nameof(ValidManaStrings))]
    public void TestGetCMC(string exp, float exp2)
    {
        var act = ManaSymbol.Create(exp);
        Assert.True(act.IsSuccess);
        var cmc = act.Value.GetCMC();
        Assert.True(cmc.IsSuccess);
        Assert.Equal(exp2, cmc.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData("Z")]
    [InlineData("W/Z")]
    [InlineData("-1")]
    [InlineData("-2/B")]
    [InlineData("ABC/B")]
    public void TestCreateInvalid(string s)
    {
        Assert.True(ManaSymbol.Create(s).IsFailure);
    }

    [Fact]
    public void TestCreateNull()
    {
        Assert.True(ManaSymbol.Create(null!).IsFailure);
    }
}
