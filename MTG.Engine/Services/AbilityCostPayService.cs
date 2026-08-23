using MTG.Core.Abilities;
using MTG.Engine.Gameplay;

namespace MTG.Engine.Services;

public class AbilityCostPayService : IAbilityCostPayService
{
    private readonly ManaPayService _manaPayService;

    public AbilityCostPayService(ManaPayService? manaPayService = null)
    {
        _manaPayService = manaPayService ?? new ManaPayService();
    }

    public bool CanPay(IAbilityCost cost, GameContext context, CardInstance source, CommanderPlayer player)
    {
        return cost switch
        {
            TapCost => !source.IsTapped,
            PayLifeCost life => player.LifeTotal > life.Amount,
            ManaCostData mana => _manaPayService.CanAfford(mana.Mana, player.ManaPool).IsSuccess,
            _ => false
        };
    }

    public void Pay(IAbilityCost cost, GameContext context, CardInstance source, CommanderPlayer player)
    {
        if (!CanPay(cost, context, source, player))
        {
            throw new InvalidOperationException("Cost cannot be paid.");
        }

        switch (cost)
        {
            case TapCost:
                source.IsTapped = true;
                break;

            case PayLifeCost life:
                player.LifeTotal -= life.Amount;
                break;

            case ManaCostData mana:
                var res = _manaPayService.TryPay(mana.Mana, player.ManaPool);
                if (res.IsFailure)
                {
                    throw new InvalidOperationException(res.Error);
                }
                player.UpdateManaPool(res.Value);
                break;
        }
    }
}