using System.Text.RegularExpressions;
using MTG.Core.Abilities;
using MTG.Core.Enums;
using MTG.Core.Helper;

namespace MTG.Core.Parser;

public class EffectParser : IEffectParser
{
    private readonly List<IEffectPatternRule> _rules;

    public EffectParser()
    {
        _rules =
        [
            // 1. Draw Cards: Optionales 's' bei draw(s) zulassen
            new EffectPatternRule(
                @"^draws?\s+(?<amount>\d+|a|an)\s+cards?$",
                match =>
                {
                    int amount = ParseNumber(match.Groups["amount"].Value);
                    return new DrawCardsEffect(amount);
                }),

            // 2. Gain Life: "you gain X life"
            new EffectPatternRule(
                @"^you\s+gain\s+(?<amount>\d+)\s+life$",
                match =>
                {
                    int amount = int.Parse(match.Groups["amount"].Value);
                    return new GainLifeEffect(amount);
                }),

            // 3. Lose Life: "you lose X life", "target player loses X life"
            new EffectPatternRule(
                @"^(?:you|target\s+player)\s+loses?\s+(?<amount>\d+)\s+life$",
                match =>
                {
                    int amount = int.Parse(match.Groups["amount"].Value);
                    return new LoseLifeEffect(amount);
                }),

            // 4. Modify Power / Toughness: "gets +X/+Y until end of turn", "gets +1/+1"
            new EffectPatternRule(
                @"^gets?\s+(?<power>[\+\-]\d+)/(?<toughness>[\+\-]\d+)(?:\s+(?<eot>until\s+end\s+of\s+turn))?$",
                match =>
                {
                    int power = int.Parse(match.Groups["power"].Value);
                    int toughness = int.Parse(match.Groups["toughness"].Value);
                    bool untilEot = match.Groups["eot"].Success;
                    return new ModifyPowerToughnessEffect(power, toughness, untilEot);
                }),

            // 5. Create Token: "create a 1/1 white Soldier creature token with lifelink"
            new EffectPatternRule(
                @"^create\s+(?<amount>\d+|a|an)\s+(?<power>\d+)/(?<toughness>\d+)\s+(?<color>\w+)\s+(?<subtype>\w+)\s+creature\s+token(?:\s+with\s+(?<keywords>.+))?$",
                match =>
                {
                    int amount = ParseNumber(match.Groups["amount"].Value);
                    int power = int.Parse(match.Groups["power"].Value);
                    int toughness = int.Parse(match.Groups["toughness"].Value);
                    string color = match.Groups["color"].Value;
                    string subtype = match.Groups["subtype"].Value;

                    var keywords = match.Groups["keywords"].Success
                        ? match.Groups["keywords"].Value.Split("and", StringSplitOptions.TrimEntries)
                        : Array.Empty<string>();

                    return new CreateTokenEffect(amount, power, toughness, color, subtype, keywords);
                }),

            // 6. Deal Damage: Optional 's' at deal(s) and optional target (to ...)
            new EffectPatternRule(
                @"^deals?\s+(?<amount>\d+)\s+damage(?:\s+to\s+(?<target>.+))?$",
                match =>
                {
                    int damage = int.Parse(match.Groups["amount"].Value);
                    string rawTarget = match.Groups["target"].Success ? match.Groups["target"].Value.ToLowerInvariant() : "any";
            
                    TargetType targetType = rawTarget switch
                    {
                        var t when t.Contains("creature") => TargetType.Creature,
                        var t when t.Contains("player") => TargetType.Player,
                        _ => TargetType.Any
                    };
            
                    return new DealDamageEffect(damage, targetType);
                }),
            
            // 7. Destroy Target: Optionales 's' bei destroy(s)
            new EffectPatternRule(
                @"^destroys?\s+target\s+(?<target>.+)$",
                match =>
                {
                    string rawTarget = match.Groups["target"].Value;
                    return new DestroyTargetEffect(CardFilter.Parse(rawTarget));
                }),

            // 8. Add Counters: "put X +1/+1 counters on target creature"
            new EffectPatternRule(
                @"^put\s+(?<amount>\d+|a|an)\s+(?<type>[\+\-\w]+)\s+counters?\s+on\s+(?<target>.+)$",
                match =>
                {
                    int amount = ParseNumber(match.Groups["amount"].Value);
                    return new AddCountersEffect(MarkerType.PlusOnePlusOne, amount);
                }),

            // 9. Discard Cards: "target player discards X cards", "discard a card"
            new EffectPatternRule(
                @"^(?:target\s+player\s+discards?|discard)\s+(?<amount>\d+|a|an)\s+cards?$",
                match =>
                {
                    int amount = ParseNumber(match.Groups["amount"].Value);
                    return new DiscardCardEffect(amount);
                }),

            // 10. Scry: "scry X"
            new EffectPatternRule(
                @"^scry\s+(?<amount>\d+)$",
                match => new ScryEffect(int.Parse(match.Groups["amount"].Value)))
        ];
    }

    public Result<IEffect> Parse(string rawEffectText)
    {
        if (string.IsNullOrWhiteSpace(rawEffectText))
            return Result<IEffect>.Failure("Effect text cannot be empty.");

        // Handle multiple effects separated by period or "and" (e.g., "You gain 1 life. Draw a card.")
        string[] effectSegments = rawEffectText
            .Split(new[] { '.', ';' }, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

        if (effectSegments.Length > 1)
        {
            var parsedSubEffects = new List<IEffect>();
            foreach (var segment in effectSegments)
            {
                var subResult = ParseSingleEffect(segment);
                if (subResult.IsSuccess)
                {
                    parsedSubEffects.Add(subResult.Value);
                }
            }

            if (parsedSubEffects.Count > 0)
                return Result<IEffect>.Success(new MultipleEffects(parsedSubEffects.AsReadOnly()));
        }

        return ParseSingleEffect(rawEffectText.Trim());
    }

    private Result<IEffect> ParseSingleEffect(string text)
    {
        // Trim end punctuation so "destroys target creature." becomes "destroys target creature"
        string normalizedText = text.TrimEnd('.').ToLowerInvariant().Trim();

        foreach (var rule in _rules)
        {
            var result = rule.TryMatch(normalizedText);
            if (result.IsSuccess)
            {
                return Result<IEffect>.Success(result.Value);
            }
        }

        return Result<IEffect>.Success(new UnhandledEffect(text));
    }

    private static int ParseNumber(string value)
    {
        return value switch
        {
            "a" or "an" => 1,
            _ => int.TryParse(value, out int result) ? result : 1
        };
    }

    private interface IEffectPatternRule
    {
        Result<IEffect> TryMatch(string text);
    }

    private class EffectPatternRule : IEffectPatternRule
    {
        private readonly Regex _regex;
        private readonly Func<Match, IEffect> _factory;

        public EffectPatternRule(string pattern, Func<Match, IEffect> factory)
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
                return Result<IEffect>.Failure($"Error parsing effect: {ex.Message}");
            }
        }
    }
}