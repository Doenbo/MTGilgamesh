using MTG.Core.Abilities;
using MTG.Core.Components;
using MTG.Core.Components.OracleText;
using MTG.Core.Enums;
using MTG.Core.Helper;
using System.Text.RegularExpressions;

namespace MTG.Core.Parser;

public partial class OracleTextParser : IOracleTextParser
{
    private readonly IAbilityCostParser _costParser;
    private readonly IEffectParser _effectParser;

    [GeneratedRegex(@"Add\s+([^\.\n]+)", RegexOptions.IgnoreCase)]
    private static partial Regex GetAddManaLineRegex();

    [GeneratedRegex(@"\{([WUBRGC0-9])\}", RegexOptions.IgnoreCase)]
    private static partial Regex GetManaSymbolRegex();

    public OracleTextParser() : this(new AbilityCostParser(), new EffectParser()) { }

    public OracleTextParser(IAbilityCostParser costParser, IEffectParser effectParser)
    {
        _costParser = costParser;
        _effectParser = effectParser;
    }

    public Result<IReadOnlyList<ICardComponent>> Parse(string oracleText, CardContext? context = null)
    {
        if (string.IsNullOrWhiteSpace(oracleText))
            return Result<IReadOnlyList<ICardComponent>>.Success([]);

        var components = new List<ICardComponent>();
        var lines = oracleText.Split('\n', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

        foreach (var line in lines)
        {
            var componentResult = ParseLine(line, context);
            if (componentResult.IsFailure)
                return componentResult.ToFailure<IReadOnlyList<ICardComponent>>();

            if (componentResult.Value is not null)
            {
                components.Add(componentResult.Value);
            }
        }

        return Result<IReadOnlyList<ICardComponent>>.Success(components.AsReadOnly());
    }

    private Result<ICardComponent?> ParseLine(string line, CardContext? context)
    {
        if (line.Contains("Add", StringComparison.OrdinalIgnoreCase))
        {
            var manaUnitsResult = ParseManaUnitsFromLine(line);

            if (manaUnitsResult.IsSuccess && manaUnitsResult.Value.Count > 0)
            {
                bool requiresTap = DetermineRequiresTap(line);

                var manaComponentResult = ProduceManaComponent.Create(manaUnitsResult.Value, requiresTap);
                if (manaComponentResult.IsFailure)
                    return manaComponentResult.ToFailure<ICardComponent?>();

                return Result<ICardComponent?>.Success(manaComponentResult.Value);
            }
        }

        if (line.Contains(':'))
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

        if (IsTriggeredLine(line))
        {
            string condition = ExtractTriggerCondition(line);
            var effectResult = _effectParser.Parse(ExtractTriggerEffect(line));

            if (effectResult.IsFailure) return effectResult.ToFailure<ICardComponent?>();

            var triggeredComponentResult = TriggeredAbilityComponent.Create(condition, effectResult.Value);
            if (triggeredComponentResult.IsFailure)
                return triggeredComponentResult.ToFailure<ICardComponent?>();

            return Result<ICardComponent?>.Success(triggeredComponentResult.Value);
        }

        return Result<ICardComponent?>.Success(null);
    }

    private Result<List<ManaUnit>> ParseManaUnitsFromLine(string line)
    {
        var manaUnits = new List<ManaUnit>();

        var dynamicResult = TryParseDynamicMana(line);
        if (dynamicResult.IsSuccess)
        {
            var unitResult = ManaUnit.CreateDynamic(dynamicResult.Value);
            if (unitResult.IsFailure)
                return unitResult.ToFailure<List<ManaUnit>>();

            manaUnits.Add(unitResult.Value);
            return Result<List<ManaUnit>>.Success(manaUnits);
        }

        var match = GetAddManaLineRegex().Match(line);
        if (!match.Success)
            return Result<List<ManaUnit>>.Failure($"Line contained 'Add' but did not match mana pattern.");

        string capturedText = match.Groups[1].Value;
        var symbolMatches = GetManaSymbolRegex().Matches(capturedText);

        var parsedTypes = new List<ManaType>();
        foreach (Match symbolMatch in symbolMatches)
        {
            string symbolCode = symbolMatch.Groups[1].Value.ToUpperInvariant();
            if (TryParseManaType(symbolCode, out var manaType))
            {
                parsedTypes.Add(manaType);
            }
        }

        if (parsedTypes.Count == 0)
            return Result<List<ManaUnit>>.Failure($"No valid mana types parsed from '{capturedText}'.");

        bool isChoicePattern = capturedText.Contains(" or ", StringComparison.OrdinalIgnoreCase);

        if (isChoicePattern)
        {
            var choiceUnitResult = ManaUnit.CreateChoice(parsedTypes);
            if (choiceUnitResult.IsFailure)
                return choiceUnitResult.ToFailure<List<ManaUnit>>();

            manaUnits.Add(choiceUnitResult.Value);
        }
        else
        {
            foreach (var type in parsedTypes)
            {
                var fixedUnitResult = ManaUnit.CreateFixed(type);
                if (fixedUnitResult.IsFailure)
                    return fixedUnitResult.ToFailure<List<ManaUnit>>();

                manaUnits.Add(fixedUnitResult.Value);
            }
        }

        return Result<List<ManaUnit>>.Success(manaUnits);
    }

    private static bool DetermineRequiresTap(string line)
    {

        if (line.Contains(':'))
        {
            string costPart = line.Split(':', 2)[0];
            return costPart.Contains("{T}", StringComparison.OrdinalIgnoreCase);
        }

        return line.Contains("{T}", StringComparison.OrdinalIgnoreCase);
    }

    private static Result<ManaDynamicType> TryParseDynamicMana(string line)
    {
        if (line.Contains("commander's color identity", StringComparison.OrdinalIgnoreCase))
            return Result<ManaDynamicType>.Success(ManaDynamicType.CommanderColorIdentity);

        if (line.Contains("opponent controls could produce", StringComparison.OrdinalIgnoreCase) ||
            line.Contains("opponents control", StringComparison.OrdinalIgnoreCase))
            return Result<ManaDynamicType>.Success(ManaDynamicType.OpponentLandColor);

        if (line.Contains("any color", StringComparison.OrdinalIgnoreCase) ||
            line.Contains("one mana of any", StringComparison.OrdinalIgnoreCase) ||
            line.Contains("two mana of any", StringComparison.OrdinalIgnoreCase) ||
            line.Contains("three mana of any", StringComparison.OrdinalIgnoreCase))
            return Result<ManaDynamicType>.Success(ManaDynamicType.AnyColor);

        return Result<ManaDynamicType>.Failure("No dynamic mana pattern matched.");
    }

    private static bool TryParseManaType(string code, out ManaType manaType)
    {
        ManaType? parsedType = code switch
        {
            "W" => ManaType.White,
            "U" => ManaType.Blue,
            "B" => ManaType.Black,
            "R" => ManaType.Red,
            "G" => ManaType.Green,
            "C" => ManaType.Colorless,
            _ => null
        };

        if (parsedType.HasValue)
        {
            manaType = parsedType.Value;
            return true;
        }

        manaType = default;
        return false;
    }

    private bool IsTriggeredLine(string line) =>
        line.StartsWith("When ", StringComparison.OrdinalIgnoreCase) ||
        line.StartsWith("Whenever ", StringComparison.OrdinalIgnoreCase) ||
        line.StartsWith("At ", StringComparison.OrdinalIgnoreCase);

    private string ExtractTriggerCondition(string line) => line.Split(',', 2)[0];
    private string ExtractTriggerEffect(string line) => line.Contains(',') ? line.Split(',', 2)[1] : line;
}