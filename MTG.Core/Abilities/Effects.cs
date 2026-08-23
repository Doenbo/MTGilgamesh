using MTG.Core.Enums;

namespace MTG.Core.Abilities;

public interface IEffect { }
public record DrawCardsEffect(int Amount) : IEffect;
public record DealDamageEffect(int Damage, TargetType Target) : IEffect;
public record DestroyTargetEffect(CardFilter Filter) : IEffect;
public record AddCountersEffect(MarkerType CounterType, int Amount) : IEffect;
public record ModifyPowerToughnessEffect(int Power, int Toughness) : IEffect;
public record MultipleEffects(IReadOnlyList<IEffect> Effects) : IEffect;
