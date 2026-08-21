using MTG.Core.Abilities;
using MTG.Engine.Gameplay;
using static MTG.Core.Abilities.AbilityCosts;

namespace MTG.Engine.Services;

public class AbilityCostPayService : IAbilityCostPayService
{
    public bool CanPay(IAbilityCost cost, GameContext context, CardInstance source, CommanderPlayer player)
    {
        var mps = new ManaPayService();
        return cost switch
        {
            TapCost => !source.IsTapped,
            PayLifeCost life => player.LifeTotal > life.Amount,
            //ManaCostData mana => mps.CanPay(mana.Mana, player.ManaPool),
            //SacrificeCost sac => context.GetPermanentsControlledBy(player).Any(p => p.HasType(sac.TargetType)),
            _ => false
        };
    }

    public void Pay(IAbilityCost cost, GameContext context, CardInstance source, CommanderPlayer player)
    {
        switch (cost)
        {
            case TapCost:
                source.IsTapped = true;
                break;
            case PayLifeCost life:
                player.LifeTotal -= life.Amount;
                break;
            case ManaCostData mana:
                var mps = new ManaPayService();
                var res = mps.TryPay(mana.Mana, player.ManaPool);
                if (res.IsFailure)
                    throw new Exception(); //TODO
                player.UpdateManaPool(res.Value);
                break;
                // ...
        }
    }
}
