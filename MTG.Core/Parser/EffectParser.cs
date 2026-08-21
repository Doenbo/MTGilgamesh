using MTG.Core.Abilities;
using MTG.Core.Helper;
using System.Text.RegularExpressions;

namespace MTG.Core.Parser;

public class EffectParser : IEffectParser
{
    private readonly List<IEffectPatternRule> _rules;

    public EffectParser()
    {
        // Initializing rules inside constructor allows invoking non-static instance methods like ParseNumber
        _rules =
        [
            // 1. "Draw X card(s)"
            new PatternRule(
                @"^draws?\s+(?<amount>\d+|a|an)\s+cards?$",
                match =>
                {
                    int amount = ParseNumber(match.Groups["amount"].Value);
                    return new DrawCardsEffect(amount);
                }),

            // 2. "deals X damage to any target / to target..."
            new PatternRule(
                @"^deals?\s+(?<damage>\d+)\s+damage(?:\s+to\s+(?<target>.+))?$",
                match =>
                {
                    int damage = int.Parse(match.Groups["damage"].Value);
                    string targetText = match.Groups["target"].Value;
                    bool isAnyTarget = string.IsNullOrEmpty(targetText) || targetText.Contains("any target");

                    return new DealDamageEffect(damage, isAnyTarget);
                }),

            // 3. "destroy target..."
            new PatternRule(
                @"^destroys?\s+target\s+(?<target>.+)$",
                match => new DestroyTargetEffect())
        ];
    }

    public Result<IEffect> Parse(string rawEffect)
    {
        if (string.IsNullOrWhiteSpace(rawEffect))
            return Result<IEffect>.Failure("Effect text cannot be empty.");

        // Normalize text: trim white spaces, strip trailing dots, and convert to lower-case for pattern matching
        string normalizedText = rawEffect.Trim().TrimEnd('.').ToLowerInvariant();

        foreach (var rule in _rules)
        {
            var result = rule.TryMatch(normalizedText);
            if (result.IsSuccess)
            {
                return Result<IEffect>.Success(result.Value);
            }
        }

        return Result<IEffect>.Failure($"Unknown or unsupported effect text: '{rawEffect}'");
    }

    private int ParseNumber(string value)
    {
        return value switch
        {
            "a" or "an" => 1,
            _ => int.TryParse(value, out int result) ? result : 1
        };
    }

    // Internal interface for clean pattern encapsulation
    private interface IEffectPatternRule
    {
        Result<IEffect> TryMatch(string text);
    }

    private class PatternRule : IEffectPatternRule
    {
        private readonly Regex _regex;
        private readonly Func<Match, IEffect> _factory;

        public PatternRule(string pattern, Func<Match, IEffect> factory)
        {
            _regex = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            _factory = factory;
        }

        public Result<IEffect> TryMatch(string text)
        {
            var match = _regex.Match(text);
            if (!match.Success)
                return Result<IEffect>.Failure("No match");

            try
            {
                var effect = _factory(match);
                return Result<IEffect>.Success(effect);
            }
            catch (Exception ex)
            {
                return Result<IEffect>.Failure($"Error building effect: {ex.Message}");
            }
        }
    }
}