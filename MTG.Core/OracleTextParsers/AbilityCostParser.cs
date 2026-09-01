using MTG.Core.Abilities;
using MTG.Core.Enums;
using MTG.Core.Helper;
using System.Text.RegularExpressions;

namespace MTG.Core.OracleTextParsers;

public class AbilityCostParser : IAbilityCostParser
{
    private readonly List<ICostPatternRule> _rules;

    public AbilityCostParser()
    {
        // Initializing rules inside the constructor allows using instance methods like ParseNumber
        _rules =
        [
            // 1. Tap symbol: {T}
            new CostPatternRule(
                @"^\{t\}$",
                _ => new TapCost()),

            // 2. Pay life: "Pay X life"
            new CostPatternRule(
                @"^pay\s+(?<amount>\d+)\s+life$",
                match => new PayLifeCost(int.Parse(match.Groups["amount"].Value))),

            // 3. Discard Hand: "Discard your hand"
            new CostPatternRule(
                @"^discard\s+your\s+hand$",
                _ => new DiscardHandCost()),

            // 4. Discard Cards: "Discard X card(s)"
            new CostPatternRule(
                @"^discard\s+(?<amount>\d+|a|an)\s+cards?$",
                match =>
                {
                    int amount = ParseNumber(match.Groups["amount"].Value);
                    return new DiscardCardCost(amount);
                }),

            // 5. Sacrifice: "Sacrifice a permanent", "Sacrifice this artifact", "Sacrifice 2 lands"
            new CostPatternRule(
                @"^sacrifice\s+(?:(?<amount>\d+|a|an)\s+)?(?<type>.+)$",
                match =>
                {
                    string rawType = match.Groups["type"].Value.Trim();
                    string amountGroup = match.Groups["amount"].Value;
                    int amount = string.IsNullOrEmpty(amountGroup) ? 1 : ParseNumber(amountGroup);

                    if (rawType.StartsWith("this", StringComparison.OrdinalIgnoreCase) ||
                        rawType.Equals("{this}", StringComparison.OrdinalIgnoreCase))
                    {
                        return new SacrificeSelfCost();
                    }

                    return new SacrificeCost(rawType, amount);
                }),

            // 6. Mana cost: e.g., "{1}{R}" or "{G}"
            new CostPatternRule(
                @"^(?:\{[0-9a-zA-Z/]+\})+$",
                match =>
                {
                    string upperMana = match.Value.ToUpperInvariant();
                    var manaCostResult = ManaCost.Create(upperMana);
                    if (manaCostResult.IsFailure)
                        throw new InvalidOperationException(manaCostResult.Error);

                    return new ManaCostData(manaCostResult.Value);
                }),

            // 7. "Tap four untapped creatures you control" / "Tap an untapped creature you control"
            new CostPatternRule(
                @"^tap\s+(?<amount>\d+|four|three|two|one|a|an)?\s*(?<filter>.+)\s+you\s+control$",
                match =>
                {
                    int amount = ParseNumber(match.Groups["amount"].Value);
                    return new TapCreaturesCost(amount, CardFilter.Parse(match.Groups["filter"].Value));
                }),
            
            // 8. "Remove a page counter from this artifact" / "Remove a -1/-1 counter from this creature"
            new CostPatternRule(
                @"^remove\s+a\s+(?<type>.+?)\s+counter\s+from\s+this",
                match =>
                {
                    string typeStr = match.Groups["type"].Value;
                    MarkerType type = typeStr.Equals("-1/-1") ? MarkerType.MinusOneMinusOne : MarkerType.Page;
                    return new RemoveCounterCost(type, 1);
                }),
            
            // 9. "Put a -1/-1 counter on this creature"
            new CostPatternRule(
                @"^put\s+a\s+(?<type>.+?)\s+counter\s+on\s+this\s+creature$",
                match =>
                {
                    return new PutCounterOnSelfCost(MarkerType.MinusOneMinusOne, 1);
                }),
        ];
    }

    public Result<IReadOnlyList<IAbilityCost>> Parse(string rawCosts)
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

            foreach (var rule in _rules)
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
                return Result<IReadOnlyList<IAbilityCost>>.Failure($"Unknown or unsupported cost segment: '{segment}'");
        }

        return Result<IReadOnlyList<IAbilityCost>>.Success(parsedCosts.AsReadOnly());
    }

    private int ParseNumber(string value)
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