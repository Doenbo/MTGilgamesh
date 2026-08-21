namespace MTG.Core.Abilities;

public record DrawCardsEffect(int Amount) : IEffect;

public record DealDamageEffect(int Damage, bool ToAnyTarget) : IEffect;

public record DestroyTargetEffect() : IEffect;
