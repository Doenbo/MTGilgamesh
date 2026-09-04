using MTG.Core.Abilities;
using MTG.Core.Helper;
using MTG.Engine.Cards;
using MTG.Engine.Gameplay;

namespace MTG.Engine.Services;

public class AbilityCostPayService : IAbilityCostPayService
{
    private readonly ManaPayService _manaPayService;

    public AbilityCostPayService(ManaPayService? manaPayService = null)
    {
        _manaPayService = manaPayService ?? new ManaPayService();
    }

    public Result CanPay(IAbilityCost cost, GameContext context, CardInstance source, CommanderPlayer player)
    {
        return cost switch
        {
            TapCost => source.IsTapped
                ? Result.Failure("Card is already tapped.")
                : Result.Success(),

            PayLifeCost life => player.LifeTotal <= life.Amount
                ? Result.Failure("Not enough life to pay cost.")
                : Result.Success(),

            ManaCostData mana => _manaPayService.CanAfford(mana.Mana, player.ManaPool).IsSuccess
                ? Result.Success()
                : Result.Failure("Not enough mana in pool."),

            _ => Result.Failure($"Unsupported cost type: {cost.GetType().Name}")
        };
    }

    public Result Pay(IAbilityCost cost, GameContext context, CardInstance source, CommanderPlayer player)
    {
        var canPayResult = CanPay(cost, context, source, player);
        if (canPayResult.IsFailure)
        {
            return canPayResult;
        }

        switch (cost)
        {
            case TapCost:
                source.IsTapped = true;
                return Result.Success();

            case PayLifeCost life:
                player.LifeTotal -= life.Amount;
                return Result.Success();

            case ManaCostData mana:
                var payResult = _manaPayService.TryPay(mana.Mana, player.ManaPool);
                if (payResult.IsFailure)
                {
                    return Result.Failure(payResult.Error);
                }

                player.UpdateManaPool(payResult.Value);
                return Result.Success();

            default:
                return Result.Failure($"Unsupported cost type: {cost.GetType().Name}");
        }
    }
}