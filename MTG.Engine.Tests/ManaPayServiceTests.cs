using Microsoft.SqlServer.Management.Smo;
using MTG.Core;
using MTG.Core.Enums;
using MTG.Core.Properties;
using MTG.Engine.Gameplay;
using MTG.Engine.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace MTG.Engine.Tests;

public class ManaPayServiceTests
{
    [Theory]
    //Single Mana
    [InlineData("{W}", 1, 0, 0, 0, 0, 0)]
    [InlineData("{U}", 0, 1, 0, 0, 0, 0)]
    [InlineData("{B}", 0, 0, 1, 0, 0, 0)]
    [InlineData("{R}", 0, 0, 0, 1, 0, 0)]
    [InlineData("{G}", 0, 0, 0, 0, 1, 0)]
    [InlineData("{C}", 0, 0, 0, 0, 0, 1)]

    //Generic Mana
    [InlineData("{1}", 1, 0, 0, 0, 0, 0)]
    [InlineData("{1}", 0, 0, 0, 0, 0, 1)]
    [InlineData("{3}", 1, 1, 1, 0, 0, 0)]
    [InlineData("{5}", 1, 1, 1, 1, 1, 1)]

    //Combinations
    [InlineData("{1}{G}", 0, 0, 0, 0, 2, 0)]
    [InlineData("{1}{W}{U}", 1, 1, 0, 0, 1, 0)]
    [InlineData("{2}{B}{B}", 0, 0, 4, 0, 0, 0)]
    [InlineData("{W}{U}{B}{R}{G}", 1, 1, 1, 1, 1, 0)]

    //Colorless
    [InlineData("{2}{R}", 0, 0, 0, 1, 0, 2)]
    [InlineData("{3}{G}", 2, 0, 0, 0, 2, 0)]

    //Big CMCs
    [InlineData("{7}", 2, 2, 2, 1, 0, 0)]
    [InlineData("{4}{G}{G}", 0, 0, 0, 0, 6, 0)]
    public void TestCanAfford(string mana, int w, int u, int b, int r, int g, int c)
    {
        var cost = ManaCost.Create(mana);
        Assert.True(cost.IsSuccess);

        var pool = new ManaPool();
        AddMana(pool, w, u, b, r, g, c);

        var mps = new ManaPayService();
        Assert.True(mps.CanAfford(cost.Value, pool).IsSuccess);
    }

    [Theory]
    [InlineData("{G}", 0, 0, 0, 0, 0, 0)]
    [InlineData("{2}", 1, 0, 0, 0, 0, 0)]

    [InlineData("{G}", 1, 0, 0, 0, 0, 0)]
    [InlineData("{W}{U}", 0, 2, 0, 0, 0, 0)]
    [InlineData("{1}{B}", 0, 0, 0, 2, 0, 0)]

    [InlineData("{2}{R}", 0, 0, 0, 1, 0, 0)]
    public void TestCantAfford(string mana, int w, int u, int b, int r, int g, int c)
    {
        var cost = ManaCost.Create(mana);
        Assert.True(cost.IsSuccess);

        var pool = new ManaPool();
        AddMana(pool, w, u, b, r, g, c);

        var mps = new ManaPayService();
        Assert.True(mps.CanAfford(cost.Value, pool).IsFailure);
    }

    private void AddMana(ManaPool pool, int w, int u, int b, int r, int g, int c)
    {
        pool.AddMana(ManaType.White, w);
        pool.AddMana(ManaType.Blue, u);
        pool.AddMana(ManaType.Black, b);
        pool.AddMana(ManaType.Red, r);
        pool.AddMana(ManaType.Green, g);
        pool.AddMana(ManaType.Colorless, c);
    }
}
