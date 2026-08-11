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
    [Theory]
    //Basics
    [InlineData(ManaType.White, 1, 0, 0, 0, 0, 0)]
    [InlineData(ManaType.Blue, 0, 1, 0, 0, 0, 0)]
    [InlineData(ManaType.Black, 0, 0, 1, 0, 0, 0)]
    [InlineData(ManaType.Red, 0, 0, 0, 1, 0, 0)]
    [InlineData(ManaType.Green, 0, 0, 0, 0, 1, 0)]
    public void TestCreateFixedValid(ManaType input, int w, int u, int b, int r, int g, int c)
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
        new object[] { new List<ManaType> { ManaType.Black, ManaType.Red }, 0, 0, 1, 1, 0, 0 }, //Badlands
        new object[] { new List<ManaType> { ManaType.White, ManaType.Blue, ManaType.Black }, 1, 1, 1, 0, 0, 0 }, //Arcane Sanctum
    };

    [Theory]
    [MemberData(nameof(ValidManaStrings))]
    public void TestCreateChoiseValid(IReadOnlyList<ManaType> input, int w, int u, int b, int r, int g, int c)
    {
        var pmc = ManaUnit.CreateChoice(input);
        Assert.True(pmc.IsSuccess);

        var produced = pmc.Value;
        Assert.False(produced.IsFixed);
        Assert.True(produced.IsChoice);
        Assert.False(produced.IsDynamic);
        Assert.Equal(w + u + b + r + g + c, produced.ManaChoice.Count);
        Assert.Equal(w, produced.ManaChoice.Count(m => m == ManaType.White));
        Assert.Equal(u, produced.ManaChoice.Count(m => m == ManaType.Blue));
        Assert.Equal(b, produced.ManaChoice.Count(m => m == ManaType.Black));
        Assert.Equal(r, produced.ManaChoice.Count(m => m == ManaType.Red));
        Assert.Equal(g, produced.ManaChoice.Count(m => m == ManaType.Green));
        Assert.Equal(c, produced.ManaChoice.Count(m => m == ManaType.Colorless));
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