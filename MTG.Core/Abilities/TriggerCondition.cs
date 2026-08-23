using MTG.Core.Enums;

namespace MTG.Core.Abilities;

public interface ITriggerCondition { }

public record EntersBattlefieldCondition(CardFilter TargetFilter) : ITriggerCondition;
public record DiesCondition(CardFilter TargetFilter) : ITriggerCondition;
public record PhaseStartCondition(TurnStep Phase, RelativePlayer Player) : ITriggerCondition;
public record AttacksCondition(CardFilter AttackerFilter) : ITriggerCondition;
public record BlocksCondition(CardFilter BlockerFilter) : ITriggerCondition;
public record DealsDamageCondition(DamageType DamageType, TargetType Receiver) : ITriggerCondition;
public record CastsSpellCondition(SpellFilter Filter) : ITriggerCondition;
public record RawTriggerCondition(string OriginalText) : ITriggerCondition;