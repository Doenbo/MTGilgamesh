using MTG.Core.Abilities;
using MTG.Core.Components;
using MTG.Core.Helper;

namespace MTG.Core.Parser;

public class OracleTextParser : IOracleTextParser
{
    public Result<IReadOnlyList<ICardComponent>> Parse(string oracleText, CardContext? context = null)
    {
        if (string.IsNullOrWhiteSpace(oracleText))
            return Result<IReadOnlyList<ICardComponent>>.Success([]);

        var components = new List<ICardComponent>();

        // Split text by line break to evaluate line by line
        var lines = oracleText.Split('\n', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

        foreach (var line in lines)
        {
            var lineResult = ParseSingleLine(line);
            if (lineResult.IsFailure)
                return lineResult.ToFailure<IReadOnlyList<ICardComponent>>();

            if (lineResult.Value is not null)
            {
                components.Add(lineResult.Value);
            }
        }

        return Result<IReadOnlyList<ICardComponent>>.Success(components.AsReadOnly());
    }

    private static Result<ICardComponent?> ParseSingleLine(string line)
    {
        // 1. Priority: Check for Mana Abilities ("{T}: Add...")
        var manaResult = ProduceManaComponent.Create(line);
        if (manaResult.IsFailure)
            return manaResult.ToFailure<ICardComponent?>();

        // If a Mana Component was successfully created, we are done with this line!
        if (manaResult.Value is not null)
            return Result<ICardComponent?>.Success(manaResult.Value);

        // 2. Priority: General Activated Abilities ("Cost : Effect")
        var activatedResult = ActivatedAbilityComponent.Create(line);
        if (activatedResult.IsFailure)
            return activatedResult.ToFailure<ICardComponent?>();

        if (activatedResult.Value is not null)
            return Result<ICardComponent?>.Success(activatedResult.Value);

        // 3. Priority: Future parsers (e.g., TriggeredAbilityParser, KeywordAbilityParser)
        // ...

        // Fallback: Line did not match any known component (e.g. flavor text or plain static text)
        return Result<ICardComponent?>.Success(null);
    }
}