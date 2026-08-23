using MTG.Core.Abilities;
using MTG.Core.Components.OracleText;
using MTG.Core.Enums;

namespace MTG.Core.Tests;

public class ProduceManaComponentTests
{
    [Fact]
    public void Create_WithValidUnits_ReturnsSuccess()
    {
        var fmana = ManaUnit.CreateFixed(ManaType.White);
        Assert.True(fmana.IsSuccess, fmana.Error);

        var units = new List<ManaUnit> { fmana.Value };

        var result = ProduceManaComponent.Create(units, requiresTap: true);
        Assert.True(result.IsSuccess, result.Error);
        Assert.NotNull(result.Value);
        Assert.True(result.Value.RequiresTap);
        Assert.Single(result.Value.ManaUnits);
        Assert.Equal(ManaType.White, result.Value.ManaUnits[0].ManaFixed);
    }

    [Fact]
    public void Create_WithEmptyUnits_ReturnsFailure()
    {
        var emptyUnits = Enumerable.Empty<ManaUnit>();

        var result = ProduceManaComponent.Create(emptyUnits, requiresTap: true);

        Assert.True(result.IsFailure);
        Assert.False(string.IsNullOrWhiteSpace(result.Error));
    }
}