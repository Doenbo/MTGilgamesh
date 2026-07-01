namespace MTG.Core.Tests;

public class ManaTests
{
    [Theory]
    [InlineData("{14}{W/U}{B}")]
    [InlineData("{1}{W} // {2}{B}")]
    public void CreateValid(string exp)
    {
        var act = Mana.Create(exp);
        Assert.True(act.IsSuccess);
        Assert.Equal(exp, act.Value.ToString());
    }
}
