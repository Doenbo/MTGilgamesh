using MTG.Core.Components;
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

    public void TestCreateFixedValid(string input, int w, int u, int b, int r, int g, int c)
    {
        var pmc = ProduceManaComponent.Create(input);
        Assert.True(pmc.IsSuccess);

        var produced = pmc.Value.FixedMana;
        Assert.Equal(w, produced.Count(m => m == ManaType.White));
        Assert.Equal(u, produced.Count(m => m == ManaType.Blue));
        Assert.Equal(b, produced.Count(m => m == ManaType.Black));
        Assert.Equal(r, produced.Count(m => m == ManaType.Red));
        Assert.Equal(g, produced.Count(m => m == ManaType.Green));
        Assert.Equal(c, produced.Count(m => m == ManaType.Colorless));
    }

    [Theory]
    [InlineData("({T}: Add {B} or {R}.)", 0, 0, 1, 1, 0, 0)] //Badlands
    public void TestCreateChoseValid(string input, int w, int u, int b, int r, int g, int c)
    {
        var pmc = ProduceManaComponent.Create(input);
        Assert.True(pmc.IsSuccess);

        var produced = pmc.Value;
        Assert.True(produced.IsChoice);
        Assert.Equal(w + u + b + r + g + c, produced.ChoseMana.Count);
        Assert.Equal(w, produced.ChoseMana.Count(m => m == ManaType.White));
        Assert.Equal(u, produced.ChoseMana.Count(m => m == ManaType.Blue));
        Assert.Equal(b, produced.ChoseMana.Count(m => m == ManaType.Black));
        Assert.Equal(r, produced.ChoseMana.Count(m => m == ManaType.Red));
        Assert.Equal(g, produced.ChoseMana.Count(m => m == ManaType.Green));
        Assert.Equal(c, produced.ChoseMana.Count(m => m == ManaType.Colorless));
    }
}