namespace MTG.Core.Tests;

public class ManaCostTests
{
    public static IEnumerable<object[]> ValidManaCostStrings =>
        new List<object[]>
        {
            new object[] { "{14}{W/U}{B}", 16 },
            new object[] { "{1}{W} // {2}{B}", 5 }, //TODO multiface?
            new object[] { "{2/B}{2/R}{2/G}", 6 },
            new object[] { "{4}{B}{B/P}", 6 },
        };

    [Theory]
    [MemberData(nameof(ValidManaCostStrings))]
    public void TestCreateValid(string exp, float _)
    {
        var act = ManaCost.Create(exp);
        Assert.True(act.IsSuccess);
        Assert.Equal(exp, act.Value.ToString());
    }

    [Theory]
    [MemberData(nameof(ValidManaCostStrings))]
    public void TestGetCMC(string exp, float exp2)
    {
        var act = ManaCost.Create(exp);
        Assert.True(act.IsSuccess);
        var cmc = act.Value.GetCMC();
        Assert.True(cmc.IsSuccess);
        Assert.Equal(exp2, cmc.Value);
    }
}
