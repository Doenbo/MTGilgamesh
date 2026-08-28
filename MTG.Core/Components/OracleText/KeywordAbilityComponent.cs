using MTG.Core.Helper;
using System;
using System.Collections.Generic;
using System.Text;

namespace MTG.Core.Components.OracleText;

public class KeywordAbilityComponent : ICardComponent
{
    public string Keyword { get; }
    public string? Parameter { get; }

    private KeywordAbilityComponent(string keyword, string? parameter)
    {
        Keyword = keyword;
        Parameter = parameter;
    }

    public static Result<KeywordAbilityComponent> Create(string keyword, string? parameter = null)
    {
        if (string.IsNullOrWhiteSpace(keyword))
            return Result<KeywordAbilityComponent>.Failure("Keyword cannot be null or empty.");

        return Result<KeywordAbilityComponent>.Success(new KeywordAbilityComponent(keyword.Trim(), parameter?.Trim()));
    }
}