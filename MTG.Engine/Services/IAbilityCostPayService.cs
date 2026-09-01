using MTG.Core.Abilities;
using MTG.Core.Helper;
using MTG.Engine.Gameplay;

namespace MTG.Engine.Services;

public interface IAbilityCostPayService
{
    public Result CanPay(IAbilityCost cost, GameContext context, CardInstance source, CommanderPlayer player);
    public Result Pay(IAbilityCost cost, GameContext context, CardInstance source, CommanderPlayer player);
}
