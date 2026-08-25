using MTG.Core.Abilities;
using MTG.Core.Components;
using MTG.Core.Components.OracleText;
using MTG.Core.Helper;

namespace MTG.Core.Parser;

public class TriggeredAbilityParser : ILineComponentParser
{
    private readonly IEffectParser _effectParser;

    public TriggeredAbilityParser(IEffectParser effectParser)
    {
        _effectParser = effectParser;
    }

    public bool CanParse(string line) =>
        line.StartsWith("When ", StringComparison.OrdinalIgnoreCase) ||
        line.StartsWith("Whenever ", StringComparison.OrdinalIgnoreCase) ||
        line.StartsWith("At ", StringComparison.OrdinalIgnoreCase);

    public Result<ICardComponent?> Parse(string line, CardContext cref)
    {
        string condition = ExtractTriggerCondition(line);
        var effectResult = _effectParser.Parse(ExtractTriggerEffect(line));

        if (effectResult.IsFailure)
            return effectResult.ToFailure<ICardComponent?>();

        var triggeredComponentResult = TriggeredAbilityComponent.Create(condition, effectResult.Value);
        if (triggeredComponentResult.IsFailure)
            return triggeredComponentResult.ToFailure<ICardComponent?>();

        return Result<ICardComponent?>.Success(triggeredComponentResult.Value);
    }

    private static string ExtractTriggerCondition(string line) => line.Split(',', 2)[0];
    private static string ExtractTriggerEffect(string line) => line.Contains(',') ? line.Split(',', 2)[1] : line;
}