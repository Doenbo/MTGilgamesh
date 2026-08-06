using MTG.Core.Components;
using MTG.Core.Decks;
using MTG.Core.Enums;
using MTG.Core.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace MTG.Core.Tests;

public class ProduceManaComponentTests
{
    [Theory]
    //Basics
    [InlineData("({T}: Add {W}.)", 1, 0, 0, 0, 0, 0)]
    [InlineData("({T}: Add {U}.)", 0, 1, 0, 0, 0, 0)]
    [InlineData("({T}: Add {B}.)", 0, 0, 1, 0, 0, 0)]
    [InlineData("({T}: Add {R}.)", 0, 0, 0, 1, 0, 0)]
    [InlineData("({T}: Add {G}.)", 0, 0, 0, 0, 1, 0)]

    [InlineData("{T}: Add {C}{C}.", 0, 0, 0, 0, 0, 2)] //Sol Ring
    [InlineData("{T}: Add {W}{U}.", 1, 1, 0, 0, 0, 0)] //Azorius Chancery

    public void TestCreateFixedValid(string input, int w, int u, int b, int r, int g, int c)
    {
        var pmc = ProduceManaComponent.Create(input);
        Assert.True(pmc.IsSuccess);

        var produced = pmc.Value;
        Assert.True(produced.IsFixed);
        Assert.False(produced.IsChoice);
        Assert.False(produced.IsDynamic);
        Assert.Equal(w, produced.FixedMana.Count(m => m == ManaType.White));
        Assert.Equal(u, produced.FixedMana.Count(m => m == ManaType.Blue));
        Assert.Equal(b, produced.FixedMana.Count(m => m == ManaType.Black));
        Assert.Equal(r, produced.FixedMana.Count(m => m == ManaType.Red));
        Assert.Equal(g, produced.FixedMana.Count(m => m == ManaType.Green));
        Assert.Equal(c, produced.FixedMana.Count(m => m == ManaType.Colorless));
    }

    //https://api.scryfall.com/cards/named?fuzzy=command+tower
    [Theory]
    [InlineData("({T}: Add {B} or {R}.)", 0, 0, 1, 1, 0, 0)] //Badlands
    public void TestCreateChoseValid(string input, int w, int u, int b, int r, int g, int c)
    {
        var pmc = ProduceManaComponent.Create(input);
        Assert.True(pmc.IsSuccess);

        var produced = pmc.Value;
        Assert.False(produced.IsFixed);
        Assert.True(produced.IsChoice);
        Assert.False(produced.IsDynamic);
        Assert.Equal(w + u + b + r + g + c, produced.ChoseMana.Count);
        Assert.Equal(w, produced.ChoseMana.Count(m => m == ManaType.White));
        Assert.Equal(u, produced.ChoseMana.Count(m => m == ManaType.Blue));
        Assert.Equal(b, produced.ChoseMana.Count(m => m == ManaType.Black));
        Assert.Equal(r, produced.ChoseMana.Count(m => m == ManaType.Red));
        Assert.Equal(g, produced.ChoseMana.Count(m => m == ManaType.Green));
        Assert.Equal(c, produced.ChoseMana.Count(m => m == ManaType.Colorless));
    }

    [Theory]
    [InlineData("{T}: Add one mana of any color in your commander's color identity.",
        DynamicManaType.CommanderColorIdentity)] //Command Tower
    [InlineData("{T}: Add one mana of any color in your commander's color identity.\nSacrifice this artifact: Draw a card.",
        DynamicManaType.CommanderColorIdentity)] //Commander's Sphere
    [InlineData("Whenever this land becomes tapped, it deals 1 damage to you.\n{T}: Add one mana of any color.",
        DynamicManaType.AnyColor)] //City of Brass
    [InlineData("{T}, Pay 1 life: Add one mana of any color.",
        DynamicManaType.AnyColor)] //Mana Confluence
    [InlineData("Flying\n{T}: Add one mana of any color.",
        DynamicManaType.AnyColor)] //Birds of Paradise
    [InlineData("{T}: Add one mana of any color that a land an opponent controls could produce.",
        DynamicManaType.OpponentLandColor)] //Exotic Orchard
    [InlineData("When this artifact enters, draw three cards.\n{T}: Add three mana of any one color.\nWhenever one or more creatures an opponent controls attack you and aren't blocked, that player draws three cards and gains control of this artifact. Untap it.",
        DynamicManaType.AnyColor)] //Coveted Jewel
    public void TestCreateDynamicValid(string input, DynamicManaType dmt)
    {
        var pmc = ProduceManaComponent.Create(input);
        Assert.True(pmc.IsSuccess);

        var produced = pmc.Value;
        Assert.False(produced.IsFixed);
        Assert.False(produced.IsChoice);
        Assert.True(produced.IsDynamic);
        Assert.Equal(dmt, produced.DynamicMana);
    }
}