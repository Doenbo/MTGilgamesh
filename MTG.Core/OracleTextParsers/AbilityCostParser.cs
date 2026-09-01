using System.Text.RegularExpressions;
using MTG.Core.Abilities;
using MTG.Core.Enums;
using MTG.Core.Helper;

namespace MTG.Core.OracleTextParsers;

public class AbilityCostParser : IAbilityCostParser
{
    private readonly List<ICostPatternRule> _rules;

    public AbilityCostParser()
    {
        _rules =
        [
            // 1. Tap symbol: {T}
            new CostPatternRule(@"^\{t\}$", _ => Result<IAbilityCost>.Success(new TapCost())),

            // 2. Pay life: "Pay X life"
            new CostPatternRule(@"^pay\s+(?<amount>\d+)\s+life$", match =>
                Result<IAbilityCost>.Success(new PayLifeCost(int.Parse(match.Groups["amount"].Value)))),

            // 3. Discard Hand: "Discard your hand"
            new CostPatternRule(@"^discard\s+your\s+hand$", _ => Result<IAbilityCost>.Success(new DiscardHandCost())),

            // 4. Discard Cards: "Discard X card(s)"
            new CostPatternRule(@"^discard\s+(?<amount>\d+|a|an)\s+cards?$", match =>
                Result<IAbilityCost>.Success(new DiscardCardCost(ParseNumber(match.Groups["amount"].Value)))),

            // 5. Sacrifice
            new CostPatternRule(@"^sacrifice\s+(?:(?<amount>\d+|a|an)\s+)?(?<type>.+)$", match =>
            {
                string rawType = match.Groups["type"].Value.Trim();
                string amountGroup = match.Groups["amount"].Value;
                int amount = string.IsNullOrEmpty(amountGroup) ? 1 : ParseNumber(amountGroup);

                IAbilityCost cost = rawType.StartsWith("this", StringComparison.OrdinalIgnoreCase) ||
                                    rawType.Equals("{this}", StringComparison.OrdinalIgnoreCase)
                    ? new SacrificeSelfCost()
                    : new SacrificeCost(rawType, amount);

                return Result<IAbilityCost>.Success(cost);
            }),

            // 6. Mana cost: Kein `throw` mehr nötig! Result wird gemappt.
            new CostPatternRule(@"^(?:\{[0-9a-zA-Z/]+\})+$", match =>
                ManaCost.Create(match.Value.ToUpperInvariant())
                    .Map(manaData => (IAbilityCost)new ManaCostData(manaData))),

            // 7. Filtered Cost: Kein `null` mehr nötig! Result wird gemappt.
            new CostPatternRule(@"^tap\s+(?<amount>\d+|four|three|two|one|a|an)?\s*(?<filter>.+)\s+you\s+control$", match =>
            {
                int amount = ParseNumber(match.Groups["amount"].Value);

                return CardFilter.Parse(match.Groups["filter"].Value)
                    .Map(filter => (IAbilityCost)new TapCreaturesCost(amount, filter));
            }),

            // 8. Remove counters
            new CostPatternRule(@"^remove\s+a\s+(?<type>.+?)\s+counter\s+from\s+this", match =>
            {
                string typeStr = match.Groups["type"].Value;
                MarkerType type = typeStr.Equals("-1/-1") ? MarkerType.MinusOneMinusOne : MarkerType.Page;
                return Result<IAbilityCost>.Success(new RemoveCounterCost(type, 1));
            }),

            // 9. Put counters
            new CostPatternRule(@"^put\s+a\s+(?<type>.+?)\s+counter\s+on\s+this\s+creature$", _ =>
                Result<IAbilityCost>.Success(new PutCounterOnSelfCost(MarkerType.MinusOneMinusOne, 1))),
        ];
    }

    public Result<IReadOnlyList<IAbilityCost>> Parse(string rawCosts)
    {
        if (string.IsNullOrWhiteSpace(rawCosts))
            return Result<IReadOnlyList<IAbilityCost>>.Failure("Cost text cannot be empty.");

        var parsedCosts = new List<IAbilityCost>();
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

    private int ParseNumber(string value) => value switch
    {
        "a" or "an" or "one" => 1,
        "two" => 2,
        "three" => 3,
        "four" => 4,
        _ => int.TryParse(value, out int result) ? result : 1
    };

    private interface ICostPatternRule
    {
        Result<IAbilityCost> TryMatch(string text);
    }

    private class CostPatternRule : ICostPatternRule
    {
        private readonly Regex _regex;
        private readonly Func<Match, Result<IAbilityCost>> _factory;

        public CostPatternRule(string pattern, Func<Match, Result<IAbilityCost>> factory)
        {
            _regex = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            _factory = factory;
        }

        public Result<IAbilityCost> TryMatch(string text)
        {
            var match = _regex.Match(text);
            if (!match.Success)
                return Result<IAbilityCost>.Failure("No match");

            // Die Factory liefert direkt ein Result<IAbilityCost> zurück!
            return _factory(match);
        }
    }
}