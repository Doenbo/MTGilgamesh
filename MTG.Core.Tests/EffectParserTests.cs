using FluentAssertions;
using MTG.Core.Abilities;
using MTG.Core.Enums;
using MTG.Core.OracleTextParsers;
using MTG.Core.Types;
namespace MTG.Core.Tests;

public class EffectParserTests
{
    public static TheoryData<string, IEffect> ValidEffectTestData => new()
    {
        // Draw card patterns
        { "Draw a card", new DrawCardsEffect(1) },
        { "Draw 2 cards", new DrawCardsEffect(2) },
        { "Draws 3 cards", new DrawCardsEffect(3) },

        // Damage patterns
        { "Deals 3 damage to any target", new DealDamageEffect(3, TargetType.Any) },
        { "Deal 1 damage", new DealDamageEffect(1, TargetType.Any) },
        { "Deals 5 damage to target creature", new DealDamageEffect(5, TargetType.Creature) },

        // Destroy patterns
        { "Destroy target permanent", new DestroyTargetEffect(CardFilter.Any) },
        { "destroys target creature.", new DestroyTargetEffect(new CardFilter(RequiredTypes: [CardType.Creature])) }
    };

    [Theory]
    [MemberData(nameof(ValidEffectTestData))]
    public void Parse_ValidString_ReturnsExpectedEffect(string input, IEffect expected)
    {
        var parser = new EffectParser();

        var result = parser.Parse(input);

        result.IsSuccess.Should().BeTrue(result.Error);
        result.Value.Should().Be(expected);
    }
}