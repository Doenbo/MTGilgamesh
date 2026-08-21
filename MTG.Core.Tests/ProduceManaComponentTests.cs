using MTG.Core.Components;
using MTG.Core.Enums;

namespace MTG.Core.Tests;

public class ProduceManaComponentTests
{
    //Basic Lands
    [Theory]
    [InlineData("({T}: Add {W}.)", ManaType.White)]
    [InlineData("({T}: Add {U}.)", ManaType.Blue)]
    [InlineData("({T}: Add {B}.)", ManaType.Black)]
    [InlineData("({T}: Add {R}.)", ManaType.Red)]
    [InlineData("({T}: Add {G}.)", ManaType.Green)]
    [InlineData("({T}: Add {C}.)", ManaType.Colorless)]
    public void TestCreateFixedSingleValid(string input, ManaType exp)
    {
        var result = ProduceManaComponent.Create(input);

        Assert.True(result.IsSuccess, result.Error);

        var produced = result.Value;
        Assert.True(produced.RequiresTap);
        Assert.Single(produced.Mana);

        var manaUnit = produced.Mana[0];
        Assert.True(manaUnit.IsFixed);
        Assert.Equal(exp, manaUnit.ManaFixed);
    }

    [Theory]
    [InlineData("{T}: Add {C}{C}.", ManaType.Colorless, ManaType.Colorless)] //Sol Ring
    [InlineData("{T}: Add {W}{U}.", ManaType.White, ManaType.Blue)] //Azorius Chancery
    public void TestCreateFixedDoubleValid(string input, ManaType exp0, ManaType exp1)
    {
        var result = ProduceManaComponent.Create(input);

        Assert.True(result.IsSuccess, result.Error);

        var produced = result.Value;
        Assert.True(produced.RequiresTap);
        Assert.Equal(2, produced.Mana.Count);

        var manaUnit0 = produced.Mana[0];
        Assert.True(manaUnit0.IsFixed);
        Assert.Equal(exp0, manaUnit0.ManaFixed);

        var manaUnit1 = produced.Mana[1];
        Assert.True(manaUnit1.IsFixed);
        Assert.Equal(exp1, manaUnit1.ManaFixed);
    }

    [Theory]
    [InlineData("{U}, {T}: Add {C}{C}{C}.", ManaType.Colorless, ManaType.Colorless, ManaType.Colorless)] //Apprentice Wizard
    public void TestCreateFixedTripleValid(string input, ManaType exp0, ManaType exp1, ManaType exp2)
    {
        var result = ProduceManaComponent.Create(input);

        Assert.True(result.IsSuccess, result.Error);

        var produced = result.Value;
        Assert.True(produced.RequiresTap);
        Assert.Equal(3, produced.Mana.Count);

        var manaUnit0 = produced.Mana[0];
        Assert.True(manaUnit0.IsFixed);
        Assert.Equal(exp0, manaUnit0.ManaFixed);

        var manaUnit1 = produced.Mana[1];
        Assert.True(manaUnit1.IsFixed);
        Assert.Equal(exp1, manaUnit1.ManaFixed);

        var manaUnit2 = produced.Mana[2];
        Assert.True(manaUnit2.IsFixed);
        Assert.Equal(exp2, manaUnit2.ManaFixed);
    }

    //Dual Lands
    [Theory]
    [InlineData("({T}: Add {B} or {R}.)", ManaType.Black, ManaType.Red)] //Badlands
    [InlineData("({T}: Add {B} or {G}.)", ManaType.Black, ManaType.Green)] //Bayou
    [InlineData("({T}: Add {R} or {W}.)", ManaType.Red, ManaType.White)] //Plateau
    [InlineData("({T}: Add {G} or {W}.)", ManaType.Green, ManaType.White)] //Savannah
    [InlineData("({T}: Add {W} or {B}.)", ManaType.White, ManaType.Black)] //Scrubland
    [InlineData("({T}: Add {R} or {G}.)", ManaType.Red, ManaType.Green)] //Taiga
    [InlineData("({T}: Add {G} or {U}.)", ManaType.Green, ManaType.Blue)] //Tropical Island
    [InlineData("({T}: Add {W} or {U}.)", ManaType.White, ManaType.Blue)] //Tundra
    [InlineData("({T}: Add {U} or {B}.)", ManaType.Blue, ManaType.Black)] //Underground Sea
    [InlineData("({T}: Add {U} or {R}.)", ManaType.Blue, ManaType.Red)] //Volcanic Island

    [InlineData("{T}: Add {W}, {U}, or {B}.", ManaType.White, ManaType.Blue, ManaType.Black)] //Arcane Sanctum
    public void TestCreateChoiceValid(string input, params ManaType[] expectedChoices)
    {
        var result = ProduceManaComponent.Create(input);

        Assert.True(result.IsSuccess, result.Error);
        var produced = result.Value;

        Assert.True(produced.RequiresTap);
        Assert.Single(produced.Mana);

        var manaUnit = produced.Mana[0];
        Assert.True(manaUnit.IsChoice);

        Assert.Equal(expectedChoices.Length, manaUnit.ManaChoice.Count);
        foreach (var expected in expectedChoices.ToList())
        {
            Assert.Contains(expected, manaUnit.ManaChoice);
        }
    }

    [Theory]
    [InlineData("{T}: Add one mana of any color in your commander's color identity.",
        ManaDynamicType.CommanderColorIdentity)] //Command Tower
    [InlineData("{T}: Add one mana of any color in your commander's color identity.\nSacrifice this artifact: Draw a card.",
        ManaDynamicType.CommanderColorIdentity)] //Commander's Sphere
    [InlineData("Whenever this land becomes tapped, it deals 1 damage to you.\n{T}: Add one mana of any color.",
        ManaDynamicType.AnyColor)] //City of Brass
    [InlineData("{T}, Pay 1 life: Add one mana of any color.",
        ManaDynamicType.AnyColor)] //Mana Confluence
    [InlineData("Flying\n{T}: Add one mana of any color.",
        ManaDynamicType.AnyColor)] //Birds of Paradise
    [InlineData("{T}: Add one mana of any color that a land an opponent controls could produce.",
        ManaDynamicType.OpponentLandColor)] //Exotic Orchard
    [InlineData("When this artifact enters, draw three cards.\n{T}: Add three mana of any one color.\nWhenever one or more creatures an opponent controls attack you and aren't blocked, that player draws three cards and gains control of this artifact. Untap it.",
        ManaDynamicType.AnyColor)] //Coveted Jewel
    public void TestCreateDynamicValid(string input, ManaDynamicType dmt)
    {
        var result = ProduceManaComponent.Create(input);

        Assert.True(result.IsSuccess, result.Error);
        var produced = result.Value;

        Assert.True(produced.RequiresTap);
        Assert.Single(produced.Mana);

        var manaUnit = produced.Mana[0];

        Assert.True(manaUnit.IsDynamic);
        Assert.Equal(dmt, manaUnit.ManaDynamic);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("Trample, Haste")]
    [InlineData("{2}, {T}: Draw a card.")]
    [InlineData("({T}: Add {Z}.)")]
    public void TestCreate_InvalidOrNonManaText_ReturnsFailure(string input)
    {
        var result = ProduceManaComponent.Create(input);

        Assert.True(result.IsFailure);
        Assert.False(string.IsNullOrWhiteSpace(result.Error));
    }
}