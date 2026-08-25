using System.Text.RegularExpressions;
using MTG.Core.Abilities;
using MTG.Core.Components;
using MTG.Core.Components.OracleText;
using MTG.Core.Enums;
using MTG.Core.Helper;

namespace MTG.Core.Parser;

public partial class ProduceManaParser : ILineComponentParser
{
    [GeneratedRegex(@"Add\s+([^\.\n]+)", RegexOptions.IgnoreCase)]
    private static partial Regex GetAddManaLineRegex();

    [GeneratedRegex(@"\{([WUBRGC0-9])\}", RegexOptions.IgnoreCase)]
    private static partial Regex GetManaSymbolRegex();

    public bool CanParse(string line) => line.Contains("Add", StringComparison.OrdinalIgnoreCase);

    public Result<ICardComponent?> Parse(string line, CardContext cref)
    {
        var manaUnitsResult = ParseManaUnitsFromLine(line);
        if (manaUnitsResult.IsFailure)
            return manaUnitsResult.ToFailure<ICardComponent?>();

        bool requiresTap = DetermineRequiresTap(line);

        var manaComponentResult = ProduceManaComponent.Create(manaUnitsResult.Value, requiresTap);
        if (manaComponentResult.IsFailure)
            return manaComponentResult.ToFailure<ICardComponent?>();

        return Result<ICardComponent?>.Success(manaComponentResult.Value);
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
}