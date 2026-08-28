using MTG.Core.Abilities;
using MTG.Core.Enums;
using MTG.Core.Helper;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace MTG.Core.Parser;

public class TriggerConditionParser : ITriggerConditionParser
{
    private readonly List<ITriggerPatternRule> _rules;

    public TriggerConditionParser()
    {
        _rules =
        [
            // 1. Enters: "When this land enters, ...", "Whenever a creature enters the battlefield"
            new TriggerPatternRule(
                @"^(?:when|whenever)\s+(?<target>.+?)\s+enters?(?:\s+the\s+battlefield)?$",
                match => new EntersBattlefieldCondition(CardFilter.Parse(match.Groups["target"].Value))),

            // 2. Dies: "Whenever target creature dies"
            new TriggerPatternRule(
                @"^(?:when|whenever)\s+(?<target>.+?)\s+dies$",
                match => new DiesCondition(CardFilter.Parse(match.Groups["target"].Value))),

            // 3. Attacks: "Whenever a creature attacks"
            new TriggerPatternRule(
                @"^(?:when|whenever)\s+(?<target>.+?)\s+attacks$",
                match => new AttacksCondition(CardFilter.Parse(match.Groups["target"].Value))),

            // 4. Phase Start: "At the beginning of your upkeep"
            new TriggerPatternRule(
                @"^at\s+the\s+beginning\s+of\s+(?<player>your|each|an\s+opponent's)?\s*(?<step>\w+)$",
                match =>
                {
                    string playerGroup = match.Groups["player"].Value.ToLowerInvariant();
                    RelativePlayer player = playerGroup switch
                    {
                        "your" => RelativePlayer.You,
                        "an opponent's" => RelativePlayer.Opponent,
                        _ => RelativePlayer.Any
                    };

                    Enum.TryParse<TurnStep>(match.Groups["step"].Value, true, out var step);
                    return new PhaseStartCondition(step, player);
                })
        ];
    }

    public Result<ITriggerCondition> Parse(string rawConditionText)
    {
        if (string.IsNullOrWhiteSpace(rawConditionText))
            return Result<ITriggerCondition>.Failure("Condition text cannot be empty.");

        string normalized = rawConditionText.TrimEnd(',').Trim().ToLowerInvariant();

        foreach (var rule in _rules)
        {
            var result = rule.TryMatch(normalized);
            if (result.IsSuccess)
            {
                return Result<ITriggerCondition>.Success(result.Value);
            }
        }

        // Fallback: Custom classes remain intact while preventing crashes
        return Result<ITriggerCondition>.Success(new RawTriggerCondition(rawConditionText));
    }

    private interface ITriggerPatternRule
    {
        Result<ITriggerCondition> TryMatch(string text);
    }

    private class TriggerPatternRule : ITriggerPatternRule
    {
        private readonly Regex _regex;
        private readonly Func<Match, ITriggerCondition> _factory;

        public TriggerPatternRule(string pattern, Func<Match, ITriggerCondition> factory)
        {
            _regex = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            _factory = factory;
        }

        public Result<ITriggerCondition> TryMatch(string text)
        {
            var match = _regex.Match(text);
            if (!match.Success)
                return Result<ITriggerCondition>.Failure("No match");

            try
            {
                var condition = _factory(match);
                return Result<ITriggerCondition>.Success(condition);
            }
            catch (Exception ex)
            {
                return Result<ITriggerCondition>.Failure($"Error parsing condition: {ex.Message}");
            }
        }
    }
}
