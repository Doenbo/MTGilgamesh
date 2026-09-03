using System.Text.RegularExpressions;
using MTG.Core.Abilities;
using MTG.Core.Enums;
using MTG.Core.Helper;

namespace MTG.Core.OracleTextParsers;

public interface ITriggerConditionParser
{
    Result<ITriggerCondition> Parse(string rawConditionText);
}

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
                match => CardFilter.Parse(match.Groups["target"].Value)
                    .Map(filter => (ITriggerCondition)new EntersBattlefieldCondition(filter))),

            // 2. Dies: "Whenever target creature dies"
            new TriggerPatternRule(
                @"^(?:when|whenever)\s+(?<target>.+?)\s+dies$",
                match => CardFilter.Parse(match.Groups["target"].Value)
                    .Map(filter => (ITriggerCondition)new DiesCondition(filter))),

            // 3. Attacks: "Whenever a creature attacks"
            new TriggerPatternRule(
                @"^(?:when|whenever)\s+(?<target>.+?)\s+attacks$",
                match => CardFilter.Parse(match.Groups["target"].Value)
                    .Map(filter => (ITriggerCondition)new AttacksCondition(filter))),

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

                    if (!Enum.TryParse<TurnStep>(match.Groups["step"].Value, true, out var step))
                    {
                        return Result<ITriggerCondition>.Failure($"Unknown turn step: '{match.Groups["step"].Value}'");
                    }

                    return Result<ITriggerCondition>.Success(new PhaseStartCondition(step, player));
                }),

            // 5. Becomes Tapped
            new TriggerPatternRule(
                @"whenever\s+(?<card>.*?)\s+becomes\s+tapped",
                match => CardFilter.Parse(match.Groups["card"].Value)
                    .Map(filter => (ITriggerCondition)new BecomesTappedCondition(filter))),
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
                return result;
            }
        }

        return Result<ITriggerCondition>.Success(new UnhandledTriggerCondition(rawConditionText));
    }

    private interface ITriggerPatternRule
    {
        Result<ITriggerCondition> TryMatch(string text);
    }

    private class TriggerPatternRule : ITriggerPatternRule
    {
        private readonly Regex _regex;
        private readonly Func<Match, Result<ITriggerCondition>> _factory;

        public TriggerPatternRule(string pattern, Func<Match, Result<ITriggerCondition>> factory)
        {
            _regex = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            _factory = factory;
        }

        public Result<ITriggerCondition> TryMatch(string text)
        {
            var match = _regex.Match(text);
            if (!match.Success)
                return Result<ITriggerCondition>.Failure("No match");

            return _factory(match);
        }
    }
}