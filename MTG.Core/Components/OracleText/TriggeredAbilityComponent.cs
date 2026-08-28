using MTG.Core.Abilities;
using MTG.Core.Helper;
using System.Diagnostics;

namespace MTG.Core.Components.OracleText;

public class TriggeredAbilityComponent : ICardComponent
{
    public ITriggerCondition Condition { get; }
    public IEffect Effect { get; }

    public TriggeredAbilityComponent(ITriggerCondition condition, IEffect effect)
    {
        Condition = condition;
        Effect = effect;
    }

    public static Result<TriggeredAbilityComponent> Create(ITriggerCondition triggerCondition, IEffect effect)
    {
        if (triggerCondition is null)
            return Result<TriggeredAbilityComponent>.Failure("Trigger condition cannot be empty.");

        if (effect is null)
            return Result<TriggeredAbilityComponent>.Failure("Effect cannot be empty.");

        return Result<TriggeredAbilityComponent>.Success(new TriggeredAbilityComponent(triggerCondition, effect));
    }
}
