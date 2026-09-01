using FluentAssertions;

namespace MTG.Core.Tests;

public class ManaCostTests
{
    public static IEnumerable<object[]> ValidManaCostStrings =>
        [
            ["{14}{W/U}{B}", 16],
            ["{1}{W} // {2}{B}", 5], //TODO multiface?
            ["{2/B}{2/R}{2/G}", 6],
            ["{4}{B}{B/P}", 6],
        ];

    [Theory]
    [MemberData(nameof(ValidManaCostStrings))]
    public void TestCreateValid(string exp, float _)
    {
        var act = ManaCost.Create(exp);
        act.IsSuccess.Should().BeTrue();
        act.Value.ToString().Should().Be(exp);
    }

    [Theory]
    [MemberData(nameof(ValidManaCostStrings))]
    public void TestGetCMC(string exp, float exp2)
    {
        var act = ManaCost.Create(exp);
        act.IsSuccess.Should().BeTrue();
        var cmc = act.Value.GetCMC();
        cmc.IsSuccess.Should().BeTrue();
        cmc.Value.Should().Be(exp2);
    }
}
