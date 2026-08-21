using MTG.Core.Abilities;
using MTG.Core.Parser;
namespace MTG.Core.Tests;

public class EffectParserTests
{
    // Provides test input cases mapped to expected IEffect record instances
    public static TheoryData<string, IEffect> ValidEffectTestData => new()
    {
        // Draw card patterns
        { "Draw a card", new DrawCardsEffect(1) },
        { "Draws 3 cards.", new DrawCardsEffect(3) },
        { "draw 2 cards", new DrawCardsEffect(2) },

        // Damage patterns
        { "Deals 3 damage to any target", new DealDamageEffect(3, true) },
        { "Deal 1 damage", new DealDamageEffect(1, true) },
        { "deals 5 damage to target creature", new DealDamageEffect(5, false) },

        // Destroy patterns
        { "Destroy target permanent", new DestroyTargetEffect() },
        { "destroys target creature.", new DestroyTargetEffect() }
    };

    [Theory]
    [MemberData(nameof(ValidEffectTestData))]
    public void Parse_ValidEffectString_ReturnsExpectedEffect(string input, IEffect expectedEffect)
    {
        // Act
        var result = new EffectParser().Parse(input);

        // Assert
        Assert.True(result.IsSuccess, $"Failed to parse valid input: '{input}'. Error: {result.Error}");
        Assert.Equal(expectedEffect, result.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("Gain 3 life")] // Unsupported pattern
    [InlineData("Invalid random text")]
    public void Parse_InvalidOrUnsupportedEffect_ReturnsFailure(string input)
    {
        // Act
        var result = new EffectParser().Parse(input);

        // Assert
        Assert.True(result.IsFailure);
        Assert.False(string.IsNullOrWhiteSpace(result.Error));
    }
}
