using FluentAssertions;
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
        fmana.IsSuccess.Should().BeTrue(fmana.Error);

        var units = new List<ManaUnit> { fmana.Value };

        var result = ProduceManaComponent.Create(units, requiresTap: true);
        result.IsSuccess.Should().BeTrue(result.Error);
        result.Value.Should().NotBeNull();
        result.Value.RequiresTap.Should().BeTrue();
        result.Value.ManaUnits.Should().ContainSingle();
        result.Value.ManaUnits[0].ManaFixed.Should().Be(ManaType.White);
    }

    [Fact]
    public void Create_WithEmptyUnits_ReturnsFailure()
    {
        var emptyUnits = Enumerable.Empty<ManaUnit>();

        var result = ProduceManaComponent.Create(emptyUnits, requiresTap: true);

        result.IsFailure.Should().BeTrue();
        string.IsNullOrWhiteSpace(result.Error).Should().BeFalse();
    }
}