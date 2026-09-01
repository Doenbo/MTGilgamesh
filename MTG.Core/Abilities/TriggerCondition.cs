using MTG.Core.Enums;

namespace MTG.Core.Abilities;

public interface ITriggerCondition { }

public record EntersBattlefieldCondition(CardFilter? TargetFilter = null) : ITriggerCondition;

public record DiesCondition(CardFilter? TargetFilter = null) : ITriggerCondition;

public record PhaseStartCondition(TurnStep Step, RelativePlayer Player = RelativePlayer.You) : ITriggerCondition;

public record AttacksCondition(CardFilter? AttackerFilter = null) : ITriggerCondition;

public record BlocksCondition(CardFilter? BlockerFilter = null) : ITriggerCondition;

public record DealsDamageCondition(DamageType DamageType = DamageType.Combat, TargetType Receiver = TargetType.Any) : ITriggerCondition;

public record CastsSpellCondition(SpellFilter? Filter = null) : ITriggerCondition;

public record BecomesTappedCondition(CardFilter? TargetFilter = null) : ITriggerCondition;

public record UnhandledTriggerCondition(string OriginalText) : ITriggerCondition;