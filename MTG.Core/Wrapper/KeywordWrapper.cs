using MTG.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace MTG.Core.Wrapper;

public readonly record struct KeywordWrapper
{
    public AbilityWord? AbilityWord { get; }
    public KeywordAbility? KeywordAbility { get; }
    public KeywordAction? KeywordAction { get; }

    public KeywordWrapper(AbilityWord type) => AbilityWord = type;
    public KeywordWrapper(KeywordAbility type) => KeywordAbility = type;
    public KeywordWrapper(KeywordAction type) => KeywordAction = type;

    public static implicit operator KeywordWrapper(AbilityWord type) => new(type);
    public static implicit operator KeywordWrapper(KeywordAbility type) => new(type);
    public static implicit operator KeywordWrapper(KeywordAction type) => new(type);

    public static bool TryParse(string rawValue, out KeywordWrapper result)
    {
        if (Enum.TryParse<AbilityWord>(rawValue, ignoreCase: true, out var abilityWord))
        {
            result = abilityWord;
            return true;
        }
        if (Enum.TryParse<KeywordAbility>(rawValue, ignoreCase: true, out var keywordAbility))
        {
            result = keywordAbility;
            return true;
        }
        if (Enum.TryParse<KeywordAction>(rawValue, ignoreCase: true, out var keywordAction))
        {
            result = keywordAction;
            return true;
        }

        result = default;
        return false;
    }
}