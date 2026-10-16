using FluentAssertions;
using MTG.Core.Abilities;
using MTG.Core.Cards;
using MTG.Core.Components.OracleText;
using MTG.Core.Enums;
using MTG.Core.Types;
using MTG.Creator;

namespace MTG.Complete.Tests;

public class TestMechanics
{
    [Fact]
    public async Task EnterTheBattlefieldValid()
    {
        var cref = new CardRef() { Name = "Wall of Omens" };
        var res = await new CardCreator().GetByExactName(cref);
        res.IsSuccess.Should().BeTrue(res.Error);

        var card = res.Value;
        card.MainFace.TryGetComponent<TriggeredAbilityComponent>(out var c1).Should().BeTrue();
        c1.Should().NotBeNull();

        c1.Condition.Should().BeEquivalentTo(new EntersBattlefieldCondition(CardFilter.Self));
        c1.Effect.Should().BeEquivalentTo(new DrawCardsEffect(1));
    }
}
