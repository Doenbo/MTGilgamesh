using FluentAssertions;
using MTG.Core.Abilities;
using MTG.Core.Cards;
using MTG.Core.Components.OracleText;
using MTG.Core.Enums;
using MTG.Core.Tests.Dummy;
using MTG.Core.Types;
using MTG.Creator;

namespace MTG.Complete.Tests;

public class TestMechanics
{
    [Fact]
    public async Task EnterTheBattlefieldValid()
    {
        var cres = CoreDummyFactory.CreatePresetCard(PresetCard.WallOfOmens);
        cres.IsSuccess.Should().BeTrue(cres.Error);

        var card = cres.Value;
        card.MainFace.TryGetComponent<TriggeredAbilityComponent>(out var c1).Should().BeTrue();
        c1.Should().NotBeNull();

        c1.Condition.Should().BeEquivalentTo(new EntersBattlefieldCondition(CardFilter.Self));
        c1.Effect.Should().BeEquivalentTo(new DrawCardsEffect(1));
    }
}
