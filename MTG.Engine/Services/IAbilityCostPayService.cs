using MTG.Core.Abilities;
using MTG.Engine.Gameplay;
using System;
using System.Collections.Generic;
using System.Text;

namespace MTG.Engine.Services;

public interface IAbilityCostPayService
{
    public bool CanPay(IAbilityCost cost, GameContext context, CardInstance source, CommanderPlayer player);
    public void Pay(IAbilityCost cost, GameContext context, CardInstance source, CommanderPlayer player);
}
