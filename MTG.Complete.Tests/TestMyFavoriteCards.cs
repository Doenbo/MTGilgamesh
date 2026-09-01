using FluentAssertions;
using MTG.Core.Abilities;
using MTG.Core.Cards;
using MTG.Core.Components;
using MTG.Core.Components.OracleText;
using MTG.Core.Enums;
using MTG.Core.OracleTextParsers;
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
        Assert.True(res.IsSuccess);

        var card = res.Value;
        AssertCardTypes(card, false, false, false, true, true, false, false, true, true, false, false);

        Assert.True(card.MainFace.TryGetComponent<CreatureComponent>(out var c1));
        Assert.Equal("2", c1.Power.Value);
        Assert.Equal("2", c1.Toughness.Value);

        Assert.True(card.MainFace.TryGetComponent<ColorComponent>(out var c2));
        Assert.Equal(ManaType.Green | ManaType.White, c2.Colors);
        Assert.Equal(ManaType.Green | ManaType.White, c2.ColorIdentity);
        Assert.Equal(ManaType.Colorless, c2.ColorIndicator); //TODO why?

        Assert.True(card.MainFace.TryGetComponent<TriggeredAbilityComponent>(out var c3));
        var cond = new BecomesTappedCondition(new CardFilter());
        var eff = new CreateTokenEffect(1, 1, 1, ManaType.White, [CreatureType.Soldier], [KeywordAbility.Lifelink]);

        c3.Condition.Should().BeEquivalentTo(cond);
        c3.Effect.Should().BeEquivalentTo(eff);


        //TODOS
        //-Results in parser und so einbauen. gibt bestimmt viele stellen!!
        //-nugets aufräumen?
        //-überall FluentAssertions verwenden?
    }

    private static void AssertCardTypes(ICard card, bool isArtifact, bool isBasic, bool isBattle, bool isCreature,
        bool isHistoric, bool isInstant, bool isLand, bool isLegendary, bool isPermanent, bool isPlaneswalker,
        bool isMultifaced)
    {
        Assert.Equal(isArtifact, card.IsArtifact());
        Assert.Equal(isBasic, card.IsBasic());
        Assert.Equal(isBattle, card.IsBattle());
        Assert.Equal(isCreature, card.IsCreature());
        Assert.Equal(isHistoric, card.IsHistoric());
        Assert.Equal(isInstant, card.IsInstant());
        Assert.Equal(isLand, card.IsLand());
        Assert.Equal(isLegendary, card.IsLegendary());
        Assert.Equal(isPermanent, card.IsPermanent());
        Assert.Equal(isPlaneswalker, card.IsPlaneswalker());
        Assert.Equal(isMultifaced, card.IsMultifaced());
    }
}
