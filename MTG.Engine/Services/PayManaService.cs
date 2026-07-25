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
    public bool CanAfford(ManaPool pool, ManaCostComponent mcComponent)
    {
        if(mcComponent.CMC > pool.TotalMana)
            return false;

        return true;
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
