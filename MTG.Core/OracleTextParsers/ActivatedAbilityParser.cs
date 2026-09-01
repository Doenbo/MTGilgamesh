using MTG.Core.Abilities;
using MTG.Core.Components;
using MTG.Core.Components.OracleText;
using MTG.Core.Helper;

namespace MTG.Core.OracleTextParsers;

public class ActivatedAbilityParser : ILineComponentParser
{
    private readonly IAbilityCostParser _costParser;
    private readonly IEffectParser _effectParser;

    public ActivatedAbilityParser(IAbilityCostParser costParser, IEffectParser effectParser)
    {
        _costParser = costParser;
        _effectParser = effectParser;
    }

    public bool CanParse(string line) => line.Contains(':');

    public Result<ICardComponent?> Parse(string line, CardContext cref)
    {
        var parts = line.Split(':', 2);
        var costResult = _costParser.Parse(parts[0]);
        var effectResult = _effectParser.Parse(parts[1]);

        if (costResult.IsFailure)
            return costResult.ToFailure<ICardComponent?>();

        if (effectResult.IsFailure)
            return effectResult.ToFailure<ICardComponent?>();

        var activatedComponentResult = ActivatedAbilityComponent.Create(costResult.Value, effectResult.Value);
        if (activatedComponentResult.IsFailure)
            return activatedComponentResult.ToFailure<ICardComponent?>();

        return Result<ICardComponent?>.Success(activatedComponentResult.Value);
    }
}