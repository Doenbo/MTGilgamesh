using MTG.Core;
using MTG.Core.Helper;
using MTG.Engine.Gameplay;

namespace MTG.Engine.Services;

public interface IManaPayService
{
    Result CanAfford(ManaCost cost, ManaPool pool);
    Result<ManaPool> TryPay(ManaCost cost, ManaPool pool);
}