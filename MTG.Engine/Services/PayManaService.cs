using MTG.Core;
using MTG.Core.Components;
using MTG.Core.Helper;
using MTG.Core.Properties;
using MTG.Engine.Gameplay;
using System;
using System.Collections.Generic;
using System.Text;

namespace MTG.Engine.Services;

public class PayManaService
{
    public bool CanPaySymbol(ManaSymbol symbol, ManaPool pool)
    {
        foreach (var color in symbol.AcceptedColors)
        {
            if (pool.Get(color) >= 1)
            {
                return true;
            }
        }

        if (symbol.GenericCost > 0 && pool.TotalMana >= symbol.GenericCost)
        {
            return true;
        }

        return false;
    }

    public bool CanPayCost(CommanderPlayer player, CardInstance card, GameContext context)
    {
        throw new NotImplementedException();
    }

    public ManaCost CalculateEffectiveCost(CardInstance card, CommanderPlayer player, GameContext context)
    {
        throw new NotImplementedException();
    }

    public Result TryPayAndCast(CommanderPlayer player, CardInstance card, GameContext context)
    {
        throw new NotImplementedException();
    }
}
