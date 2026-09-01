using MTG.Core.Enums;
using MTG.Core.Wrapper;
using System.Collections.Immutable;
using System.Data;

namespace MTG.Core.Abilities;

public interface IEffect { }
public record DrawCardsEffect(int Amount) : IEffect;

public record DealDamageEffect(int Damage, TargetType Target) : IEffect;

public record DestroyTargetEffect(CardFilter Filter) : IEffect;

public record AddCountersEffect(MarkerType CounterType, int Amount) : IEffect;

public record ModifyPowerToughnessEffect(
    int Power,
    int Toughness,
    bool UntilEndOfTurn = true
) : IEffect;

public record MultipleEffects(IReadOnlyList<IEffect> Effects) : IEffect;

public record CreateTokenEffect(
    int Amount,
    int Power,
    int Toughness,
    ManaType Color,
    ImmutableArray<SubtypeWrapper> Subtypes,
    ImmutableArray<KeywordWrapper> Keywords
) : IEffect;

public record GainLifeEffect(int Amount) : IEffect;

public record LoseLifeEffect(int Amount) : IEffect;

public record DiscardCardEffect(int Amount, TargetType Target = TargetType.TargetPlayer) : IEffect;

public record ScryEffect(int Amount) : IEffect;

public record MillEffect(int Amount, TargetType Target = TargetType.TargetPlayer) : IEffect;

public record ExileTargetEffect(CardFilter Filter) : IEffect;

public record ReturnToHandEffect(CardFilter Filter) : IEffect;

public record SearchLibraryEffect(CardFilter Filter, ZoneType Destination = ZoneType.Hand) : IEffect;

public record TapTargetEffect(CardFilter Filter) : IEffect;

public record UntapTargetEffect(CardFilter Filter) : IEffect;

public record GainKeywordEffect(string Keyword, CardFilter Filter, bool UntilEndOfTurn = true) : IEffect;

public record UnhandledEffect(string RawText) : IEffect;
