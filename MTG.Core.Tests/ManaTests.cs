namespace MTG.Core.Tests;

public class ManaTests
{
    public static IEnumerable<object[]> ValidManaStrings =>
        new List<object[]>
        {
            new object[] { "{14}{W/U}{B}", 16 },
            new object[] { "{1}{W} // {2}{B}", 5 }, //TODO multiface?
            new object[] { "{2/B}{2/R}{2/G}", 6 },
            new object[] { "{4}{B}{B/P}", 6 },
        };

    [Theory]
    [MemberData(nameof(ValidManaStrings))]
    public void TestCreateValid(string exp, float _)
    {
        var act = Mana.Create(exp);
        Assert.True(act.IsSuccess);
        Assert.Equal(exp, act.Value.ToString());
    }

    [Theory]
    [MemberData(nameof(ValidManaStrings))]
    public void TestGetCMC(string exp, float exp2)
    {
        var act = Mana.Create(exp);
        Assert.True(act.IsSuccess);
        var cmc = act.Value.GetCMC();
        Assert.True(cmc.IsSuccess);
        Assert.Equal(exp2, cmc.Value);
    }
}
