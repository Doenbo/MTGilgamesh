using MTG.Core;
using MTG.Core.Abilities;
using MTG.Core.Helper;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using static MTG.Core.Abilities.AbilityCosts;

namespace MTG.Engine.Parser;

public class AbilityCostParser
{
    // Pipeline of known cost patterns (Pattern -> Cost Factory)
    private static readonly List<ICostPatternRule> Rules = new()
    {
        // 1. Tap symbol: {T}
        new CostPatternRule(
            @"^\{t\}$",
            _ => new TapCost()),

        // 2. Pay life: "Pay X life"
        new CostPatternRule(
            @"^pay\s+(?<amount>\d+)\s+life$",
            match => new PayLifeCost(int.Parse(match.Groups["amount"].Value))),

        // 3. Discard: "Discard X card(s)"
        new CostPatternRule(
            @"^discard\s+(?<amount>\d+|a|an)\s+cards?$",
            match =>
            {
                int amount = ParseNumber(match.Groups["amount"].Value);
                return new DiscardCost(amount);
            }),

        // 4. Sacrifice: "Sacrifice a permanent/creature/land..."
        new CostPatternRule(
            @"^sacrifice\s+(?:a|an)\s+(?<type>.+)$",
            match => new SacrificeCost(match.Groups["type"].Value.Trim())),

        // 5. Mana cost: e.g., "{1}{R}" or "{G}"
        new CostPatternRule(
            @"^(?:\{[0-9a-zA-Z/]+\})+$",
            match =>
            {
                // Convert to canonical upper-case representation
                string upperMana = match.Value.ToUpperInvariant();
                var manaCostResult = ManaCost.Create(upperMana);
                if (manaCostResult.IsFailure)
                    throw new InvalidOperationException(manaCostResult.Error);
        
                return new ManaCostData(manaCostResult.Value);
            })
    };

    public static Result<IReadOnlyList<IAbilityCost>> Parse(string rawCosts)
    {
        if (string.IsNullOrWhiteSpace(rawCosts))
            return Result<IReadOnlyList<IAbilityCost>>.Failure("Cost text cannot be empty.");

        var parsedCosts = new List<IAbilityCost>();

        // Split comma-separated cost components (e.g., "{1}{R}, {T}, Pay 2 life")
        string[] costSegments = rawCosts.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

        foreach (var segment in costSegments)
        {
            string normalizedSegment = segment.ToLowerInvariant();
            bool matched = false;

            foreach (var rule in Rules)
            {
                var result = rule.TryMatch(normalizedSegment);
                if (result.IsSuccess)
                {
                    parsedCosts.Add(result.Value);
                    matched = true;
                    break;
                }
            }

            if (!matched)
            {
                return Result<IReadOnlyList<IAbilityCost>>.Failure($"Unknown or unsupported cost segment: '{segment}'");
            }
        }

        return Result<IReadOnlyList<IAbilityCost>>.Success(parsedCosts.AsReadOnly());
    }

    private static int ParseNumber(string value)
    {
        return value switch
        {
            "a" or "an" => 1,
            _ => int.TryParse(value, out int result) ? result : 1
        };
    }

    private interface ICostPatternRule
    {
        Result<IAbilityCost> TryMatch(string text);
    }

    private class CostPatternRule : ICostPatternRule
    {
        private readonly Regex _regex;
        private readonly Func<Match, IAbilityCost> _factory;

        public CostPatternRule(string pattern, Func<Match, IAbilityCost> factory)
        {
            _regex = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            _factory = factory;
        }

        public Result<IAbilityCost> TryMatch(string text)
        {
            var match = _regex.Match(text);
            if (!match.Success)
                return Result<IAbilityCost>.Failure("No match");

            try
            {
                var cost = _factory(match);
                return Result<IAbilityCost>.Success(cost);
            }
            catch (Exception ex)
            {
                return Result<IAbilityCost>.Failure($"Error parsing cost: {ex.Message}");
            }
        }
    }
}