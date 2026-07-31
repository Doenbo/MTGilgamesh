using MTG.Core;
using MTG.Core.Components;
using MTG.Core.Helper;
using MTG.Core.Properties;
using MTG.Engine.Gameplay;
using System;
using System.Collections.Generic;
using System.Text;

namespace MTG.Engine.Services;

public class ManaPayService
{
    public Result CanAfford(ManaCost cost, ManaPool pool)
    {
        var payResult = TryPay(cost, pool);
        return payResult.IsSuccess ? Result.Success() : Result.Failure(payResult.Error);
    }

    public Result<ManaPool> TryPay(ManaCost cost, ManaPool pool)
    {
        var cmc = cost.GetCMC();
        if (cmc.IsFailure)
            return Result<ManaPool>.Failure(cmc.Error);

        if (pool.TotalMana < cmc.Value)
            return Result<ManaPool>.Failure($"Not enough total mana! Required: {cmc.Value}, Available: {pool.TotalMana}");

        var tempPool = pool.Clone();

        foreach (var symbol in cost.Symbols.Where(s => !s.IsGenericOnly))
        {
            bool paid = false;
            foreach (var color in symbol.AcceptedColors)
            {
                if (tempPool.TryDeduct(color, 1))
                {
                    paid = true;
                    break;
                }
            }

            if (!paid)
                return Result<ManaPool>.Failure($"Missing required mana color for symbol '{symbol}'.");
        }

        int genericAmount = cost.Symbols.Where(s => s.IsGenericOnly).Sum(s => s.GenericCost);
        if (genericAmount > 0)
        {
            if (!tempPool.TryDeductGeneric(genericAmount))
                return Result<ManaPool>.Failure("Not enough mana left to pay generic costs.");
        }

        return Result<ManaPool>.Success(tempPool);
    }
}
