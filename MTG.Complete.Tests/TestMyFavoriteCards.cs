using FluentAssertions;
using MTG.Core.Abilities;
using MTG.Core.Cards;
using MTG.Core.Components;
using MTG.Core.Components.OracleText;
using MTG.Core.Enums;
using MTG.Core.Types;
using MTG.Engine.Factories;

namespace MTG.Complete.Tests;

public class TestMyFavoriteCards
{
    [Fact]
    public async Task CreateEmmaraValid()
    {
        var cref = new CardRef() { Name = "Emmara, Soul of the Accord" };
        var res = await CardCreator.GetExact(cref);
        res.IsSuccess.Should().BeTrue();

        var card = res.Value;
        AssertCardTypes(card, false, false, false, true, true, false, false, true, true, false, false);

        card.MainFace.TryGetComponent<CreatureComponent>(out var c1).Should().BeTrue();
        c1.Should().NotBeNull();
        c1.Power.Value.Should().Be("2");
        c1.Toughness.Value.Should().Be("2");

        card.MainFace.TryGetComponent<ColorComponent>(out var c2).Should().BeTrue();
        c2.Should().NotBeNull();
        c2.Colors.Should().Be(ManaType.Green | ManaType.White);
        c2.ColorIdentity.Should().Be(ManaType.Green | ManaType.White);
        c2.ColorIndicator.Should().Be(ManaType.Colorless); //TODO why?

        card.MainFace.TryGetComponent<TriggeredAbilityComponent>(out var c3).Should().BeTrue();
        c3.Should().NotBeNull();
        c3.Condition.Should().BeEquivalentTo(
            new BecomesTappedCondition(new CardFilter()));
        c3.Effect.Should().BeEquivalentTo(
            new CreateTokenEffect(1, 1, 1, ManaType.White, [CreatureType.Soldier], [KeywordAbility.Lifelink]));
    }

    private static void AssertCardTypes(ICard card, bool isArtifact, bool isBasic, bool isBattle, bool isCreature,
        bool isHistoric, bool isInstant, bool isLand, bool isLegendary, bool isPermanent, bool isPlaneswalker,
        bool isMultifaced)
    {
        card.Should().NotBeNull();
        card.IsArtifact().Should().Be(isArtifact);
        card.IsBasic().Should().Be(isBasic);
        card.IsBattle().Should().Be(isBattle);
        card.IsCreature().Should().Be(isCreature);
        card.IsHistoric().Should().Be(isHistoric);
        card.IsInstant().Should().Be(isInstant);
        card.IsLand().Should().Be(isLand);
        card.IsLegendary().Should().Be(isLegendary);
        card.IsPermanent().Should().Be(isPermanent);
        card.IsPlaneswalker().Should().Be(isPlaneswalker);
        card.IsMultifaced().Should().Be(isMultifaced);
    }
}
