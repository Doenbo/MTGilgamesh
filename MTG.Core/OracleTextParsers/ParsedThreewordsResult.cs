using MTG.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace MTG.Core.OracleTextParsers;

public record ParsedThreewordsResult(
    IReadOnlyList<KeywordAbility> KeywordAbilities,
    IReadOnlyList<KeywordAction> KeywordActions,
    IReadOnlyList<AbilityWord> AbilityWords
);