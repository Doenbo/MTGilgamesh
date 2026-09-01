using FluentAssertions;
using MTG.Core.Abilities;
using MTG.Core.OracleTextParsers;

namespace MTG.Core.Tests;

public class AbilityCostParserTests
{
    public static TheoryData<string, IReadOnlyList<IAbilityCost>> ValidCostTestData => new()
    {
        // Single costs
        { "{T}", new List<IAbilityCost> { new TapCost() } },
        { "Pay 2 life", new List<IAbilityCost> { new PayLifeCost(2) } },
        { "Discard a card", new List<IAbilityCost> { new DiscardCardCost(1) } },

        // Multiple comma-separated costs
        {
            "{1}{R}, {T}",
            new List<IAbilityCost>
            {
                new ManaCostData(ManaCost.Create("{1}{R}").Value),
                new TapCost()
            }
        },
        {
            "{T}, Pay 3 life, Sacrifice a creature",
            new List<IAbilityCost>
            {
                new TapCost(),
                new PayLifeCost(3),
                new SacrificeCost("creature")
            }
        }
    };

    [Theory]
    [MemberData(nameof(ValidCostTestData))]
    public void Parse_ValidCostString_ReturnsExpectedCosts(string input, IReadOnlyList<IAbilityCost> expectedCosts)
    {
        var result = new AbilityCostParser().Parse(input);

        result.IsSuccess.Should().BeTrue($"Failed to parse input: '{input}'. Error: {result.Error}");
        result.Value.Count.Should().Be(expectedCosts.Count);

        for (int i = 0; i < expectedCosts.Count; i++)
        {
            Assert.Equal(expectedCosts[i], result.Value[i]);
        }
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("{T}, Pay invalid cost")]
    public void Parse_InvalidCost_ReturnsFailure(string input)
    {
        var result = new AbilityCostParser().Parse(input);

        result.IsFailure.Should().BeTrue();
        string.IsNullOrWhiteSpace(result.Error).Should().BeFalse();
    }
}
