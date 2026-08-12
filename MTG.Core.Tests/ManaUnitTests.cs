using MTG.Core.Abilities;
using MTG.Core.Components;
using MTG.Core.Decks;
using MTG.Core.Enums;
using MTG.Core.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MTG.Core.Tests;

public class ManaUnitTests
{
    //Basic Lands
    [Theory]
    [InlineData(ManaType.White)]
    [InlineData(ManaType.Blue)]
    [InlineData(ManaType.Black)]
    [InlineData(ManaType.Red)]
    [InlineData(ManaType.Green)]
    public void TestCreateFixedValid(ManaType input)
    {
        var pmc = ManaUnit.CreateFixed(input);
        Assert.True(pmc.IsSuccess);

        var produced = pmc.Value;
        Assert.True(produced.IsFixed);
        Assert.False(produced.IsChoice);
        Assert.False(produced.IsDynamic);

        Assert.Equal(input, produced.ManaFixed);
        Assert.Equal(ManaRestriction.None, produced.ManaRestriction);
    }

    public static IEnumerable<object[]> ValidManaStrings =>
    new List<object[]>
    {
        new object[] { new List<ManaType> { ManaType.Black, ManaType.Red } }, //Badlands
        new object[] { new List<ManaType> { ManaType.White, ManaType.Blue, ManaType.Black } }, //Arcane Sanctum
    };

    [Theory]
    [MemberData(nameof(ValidManaStrings))]
    public void TestCreateChoiseValid(IReadOnlyList<ManaType> input)
    {
        var pmc = ManaUnit.CreateChoice(input);
        Assert.True(pmc.IsSuccess);

        var produced = pmc.Value;
        Assert.False(produced.IsFixed);
        Assert.True(produced.IsChoice);
        Assert.False(produced.IsDynamic);

        Assert.Equal(input.Count, produced.ManaChoice.Count);
        Assert.Equal(ManaRestriction.None, produced.ManaRestriction);
    }

    [Theory]
    [InlineData(ManaDynamicType.CommanderColorIdentity)] //Command Tower
    [InlineData(ManaDynamicType.AnyColor)] //City of Brass
    [InlineData(ManaDynamicType.OpponentLandColor)] //Exotic Orchard
    public void TestCreateDynamicValid(ManaDynamicType input)
    {
        var pmc = ManaUnit.CreateDynamic(input);
        Assert.True(pmc.IsSuccess);

        var produced = pmc.Value;
        Assert.False(produced.IsFixed);
        Assert.False(produced.IsChoice);
        Assert.True(produced.IsDynamic);

        Assert.Equal(input, produced.ManaDynamic);
    }
}