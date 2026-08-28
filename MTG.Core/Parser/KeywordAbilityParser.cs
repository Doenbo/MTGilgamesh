using MTG.Core.Abilities;
using MTG.Core.Components;
using MTG.Core.Components.OracleText;
using MTG.Core.Helper;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace MTG.Core.Parser;

public class KeywordAbilityParser : ILineComponentParser
{
    private static readonly Regex KeywordRegex = new(
        @"^(?<keyword>Equip|Cycling|Basic landcycling|Unearth|Kicker|Multikicker)\s*(?<param>\{[^}]+\}|\d+)?(?:\s*\(.*\))?$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public bool CanParse(string line) => KeywordRegex.IsMatch(line.Trim());

    public Result<ICardComponent?> Parse(string line, CardContext cref)
    {
        var match = KeywordRegex.Match(line.Trim());
        if (!match.Success)
            return Result<ICardComponent?>.Failure("Not a valid keyword ability line.");

        string keyword = match.Groups["keyword"].Value;
        string parameter = match.Groups["param"].Value;

        var result = KeywordAbilityComponent.Create(keyword, parameter);
        if (result.IsFailure)
            return result.ToFailure<ICardComponent?>();

        return Result<ICardComponent?>.Success(result.Value);
    }
}