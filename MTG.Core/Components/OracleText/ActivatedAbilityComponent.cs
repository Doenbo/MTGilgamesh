using MTG.Core.Abilities;
using MTG.Core.Helper;

namespace MTG.Core.Components.OracleText;

public class ActivatedAbilityComponent : ICardComponent
{
    public IReadOnlyList<IAbilityCost> Costs { get; }
    public IEffect Effect { get; }

    private ActivatedAbilityComponent(IEnumerable<IAbilityCost> costs, IEffect effect)
    {
        Costs = costs.ToList().AsReadOnly();
        Effect = effect;
    }

    public static Result<ActivatedAbilityComponent> Create(IEnumerable<IAbilityCost> costs, IEffect effect)
    {
        var costList = costs.ToList();
        if (costList.Count == 0)
            return Result<ActivatedAbilityComponent>.Failure("Activated ability requires at least one cost.");

        if (effect is null)
            return Result<ActivatedAbilityComponent>.Failure("Activated ability requires an effect.");

        return Result<ActivatedAbilityComponent>.Success(new ActivatedAbilityComponent(costList, effect));
    }
}