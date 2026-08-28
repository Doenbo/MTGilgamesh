using MTG.Core.Abilities;
using MTG.Core.Components;
using MTG.Core.Components.OracleText;
using MTG.Core.Helper;

namespace MTG.Core.Parser;

public class TriggeredAbilityParser : ILineComponentParser
{
    private readonly ITriggerConditionParser _conditionParser;
    private readonly IEffectParser _effectParser;

    public TriggeredAbilityParser(ITriggerConditionParser conditionParser, IEffectParser effectParser)
    {
        _conditionParser = conditionParser;
        _effectParser = effectParser;
    }

    public bool CanParse(string line) =>
        line.StartsWith("When ", StringComparison.OrdinalIgnoreCase) ||
        line.StartsWith("Whenever ", StringComparison.OrdinalIgnoreCase) ||
        line.StartsWith("At ", StringComparison.OrdinalIgnoreCase);

    public Result<ICardComponent?> Parse(string line, CardContext cref)
    {
        string rawCondition = ExtractTriggerCondition(line);
        string rawEffect = ExtractTriggerEffect(line);

        var conditionResult = _conditionParser.Parse(rawCondition);
        if (conditionResult.IsFailure)
            return conditionResult.ToFailure<ICardComponent?>();

        var effectResult = _effectParser.Parse(rawEffect);
        if (effectResult.IsFailure)
            return effectResult.ToFailure<ICardComponent?>();

        var triggeredComponentResult = TriggeredAbilityComponent.Create(conditionResult.Value, effectResult.Value);
        if (triggeredComponentResult.IsFailure)
            return triggeredComponentResult.ToFailure<ICardComponent?>();

        return Result<ICardComponent?>.Success(triggeredComponentResult.Value);
    }

    private static string ExtractTriggerCondition(string line) =>
        line.Contains(',') ? line.Split(',', 2)[0].Trim() : line.Trim();

    private static string ExtractTriggerEffect(string line) =>
        line.Contains(',') ? line.Split(',', 2)[1].Trim() : string.Empty;
}