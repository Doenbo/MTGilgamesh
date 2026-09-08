using MTG.Core.Enums;

namespace MTG.Core.OracleTextParsers;

public record ParsedThreewordsResult(
    IReadOnlyList<KeywordAbility> KeywordAbilities,
    IReadOnlyList<KeywordAction> KeywordActions,
    IReadOnlyList<AbilityWord> AbilityWords
);