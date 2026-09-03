using System.Collections.Immutable;
using System.Text.RegularExpressions;
using MTG.Core.Abilities;
using MTG.Core.Enums;
using MTG.Core.Helper;
using MTG.Core.Types;
using MTG.Core.Wrapper;

namespace MTG.Core.OracleTextParsers;

public interface IEffectParser
{
    public Result<IEffect> Parse(string rawEffect);
}

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
                match => Result<IEffect>.Success(new DrawCardsEffect(ParseNumber(match.Groups["amount"].Value)))),

            // 2. Gain Life: "you gain X life"
            new EffectPatternRule(
                @"^you\s+gain\s+(?<amount>\d+)\s+life$",
                match => Result<IEffect>.Success(new GainLifeEffect(int.Parse(match.Groups["amount"].Value)))),

            // 3. Lose Life: "you lose X life", "target player loses X life"
            new EffectPatternRule(
                @"^(?:you|target\s+player)\s+loses?\s+(?<amount>\d+)\s+life$",
                match => Result<IEffect>.Success(new LoseLifeEffect(int.Parse(match.Groups["amount"].Value)))),

            // 4. Modify Power / Toughness: "gets +X/+Y until end of turn", "gets +1/+1"
            new EffectPatternRule(
                @"^gets?\s+(?<power>[\+\-]\d+)/(?<toughness>[\+\-]\d+)(?:\s+(?<eot>until\s+end\s+of\s+turn))?$",
                match =>
                {
                    int power = int.Parse(match.Groups["power"].Value);
                    int toughness = int.Parse(match.Groups["toughness"].Value);
                    bool untilEot = match.Groups["eot"].Success;
                    return Result<IEffect>.Success(new ModifyPowerToughnessEffect(power, toughness, untilEot));
                }),

            // 5. Create Token: Saubere Behandlung von Parse-Fehlern ohne `null`
            new EffectPatternRule(
                @"^create\s+(?<amount>\d+|a|an)\s+(?<power>\d+)/(?<toughness>\d+)\s+(?<color>\w+)\s+(?<subtype>\w+)\s+creature\s+token(?:\s+with\s+(?<keywords>.+))?$",
                match =>
                {
                    int amount = ParseNumber(match.Groups["amount"].Value);
                    int power = int.Parse(match.Groups["power"].Value);
                    int toughness = int.Parse(match.Groups["toughness"].Value);

                    if (!Enum.TryParse<ManaType>(match.Groups["color"].Value, ignoreCase: true, out var manaType))
                    {
                        return Result<IEffect>.Failure($"Invalid mana type: '{match.Groups["color"].Value}'");
                    }

                    if (!SubtypeWrapper.TryParse(match.Groups["subtype"].Value, out var subtype))
                    {
                        return Result<IEffect>.Failure($"Invalid subtype: '{match.Groups["subtype"].Value}'");
                    }

                    var keywords = new List<KeywordWrapper>();
                    if (match.Groups["keywords"].Success)
                    {
                        var rawKeywords = match.Groups["keywords"].Value
                            .Split([",", " and "], StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

                        foreach (var raw in rawKeywords)
                        {
                            if (KeywordWrapper.TryParse(raw, out var kw))
                            {
                                keywords.Add(kw);
                            }
                        }
                    }

                    return Result<IEffect>.Success(new CreateTokenEffect(
                        amount,
                        power,
                        toughness,
                        manaType,
                        [subtype],
                        [..keywords]
                    ));
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

                    return Result<IEffect>.Success(new DealDamageEffect(damage, targetType));
                }),
            
            // 7. Destroy Target: CardFilter-Result direkt über `IsSuccess` oder Extension-Map weiterleiten
            new EffectPatternRule(
                @"^destroys?\s+target\s+(?<target>.+)$",
                match =>
                {
                    var filterResult = CardFilter.Parse(match.Groups["target"].Value);
                    return filterResult.IsSuccess
                        ? Result<IEffect>.Success(new DestroyTargetEffect(filterResult.Value))
                        : Result<IEffect>.Failure(filterResult.Error);
                }),

            // 8. Add Counters: "put X +1/+1 counters on target creature"
            new EffectPatternRule(
                @"^put\s+(?<amount>\d+|a|an)\s+(?<type>[\+\-\w]+)\s+counters?\s+on\s+(?<target>.+)$",
                match => Result<IEffect>.Success(new AddCountersEffect(MarkerType.PlusOnePlusOne, ParseNumber(match.Groups["amount"].Value)))),

            // 9. Discard Cards: "target player discards X cards", "discard a card"
            new EffectPatternRule(
                @"^(?:target\s+player\s+discards?|discard)\s+(?<amount>\d+|a|an)\s+cards?$",
                match => Result<IEffect>.Success(new DiscardCardEffect(ParseNumber(match.Groups["amount"].Value)))),

            // 10. Scry: "scry X"
            new EffectPatternRule(
                @"^scry\s+(?<amount>\d+)$",
                match => Result<IEffect>.Success(new ScryEffect(int.Parse(match.Groups["amount"].Value))))
        ];
    }

    public Result<IEffect> Parse(string rawEffectText)
    {
        if (string.IsNullOrWhiteSpace(rawEffectText))
            return Result<IEffect>.Failure("Effect text cannot be empty.");

        string[] effectSegments = rawEffectText
            .Split(['.', ';'], StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

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
                else
                {
                    return Result<IEffect>.Failure($"Failed to parse sub-effect: '{segment}'");
                }
            }

            if (parsedSubEffects.Count > 0)
                return Result<IEffect>.Success(new MultipleEffects(parsedSubEffects.AsReadOnly()));
        }

        return ParseSingleEffect(rawEffectText.Trim());
    }

    private Result<IEffect> ParseSingleEffect(string text)
    {
        string normalizedText = text.TrimEnd('.').ToLowerInvariant().Trim();

        foreach (var rule in _rules)
        {
            var result = rule.TryMatch(normalizedText);
            if (result.IsSuccess)
            {
                return result;
            }
        }

        return Result<IEffect>.Success(new UnhandledEffect(text));
    }

    private static int ParseNumber(string value) => value switch
    {
        "a" or "an" => 1,
        _ => int.TryParse(value, out int result) ? result : 1
    };

    private interface IEffectPatternRule
    {
        Result<IEffect> TryMatch(string text);
    }

    private class EffectPatternRule : IEffectPatternRule
    {
        private readonly Regex _regex;
        private readonly Func<Match, Result<IEffect>> _factory;

        public EffectPatternRule(string pattern, Func<Match, Result<IEffect>> factory)
        {
            _regex = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            _factory = factory;
        }

        public Result<IEffect> TryMatch(string text)
        {
            var match = _regex.Match(text);
            if (!match.Success)
                return Result<IEffect>.Failure("No match");

            return _factory(match);
        }
    }
}