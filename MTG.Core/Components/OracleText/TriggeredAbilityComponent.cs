using MTG.Core.Abilities;
using MTG.Core.Helper;

namespace MTG.Core.Components.OracleText;

public class TriggeredAbilityComponent : ICardComponent
{
    public string TriggerCondition { get; }
    public IEffect Effect { get; }

    private TriggeredAbilityComponent(string triggerCondition, IEffect effect)
    {
        TriggerCondition = triggerCondition;
        Effect = effect;
    }

    public static Result<TriggeredAbilityComponent> Create(string triggerCondition, IEffect effect)
    {
        if (string.IsNullOrWhiteSpace(triggerCondition))
            return Result<TriggeredAbilityComponent>.Failure("Trigger condition cannot be empty.");

        if (effect is null)
            return Result<TriggeredAbilityComponent>.Failure("Triggered ability requires an effect.");

        return Result<TriggeredAbilityComponent>.Success(new TriggeredAbilityComponent(triggerCondition, effect));
    }
}
