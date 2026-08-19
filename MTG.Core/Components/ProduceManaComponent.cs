using MTG.Core.Abilities;
using MTG.Core.Cards;
using MTG.Core.Enums;
using MTG.Core.Helper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace MTG.Core.Components;

public partial class ProduceManaComponent : ICardComponent
{
    [GeneratedRegex(@"Add\s+([^\.\n]+)", RegexOptions.IgnoreCase)]
    private static partial Regex GetAddManaLineRegex();

    [GeneratedRegex(@"\{([WUBRGC0-9])\}", RegexOptions.IgnoreCase)]
    private static partial Regex GetManaSymbolRegex();


    private readonly List<ManaUnit> _mana = [];
    public IReadOnlyList<ManaUnit> Mana => _mana;

    public bool RequiresTap { get; private set; } = true;

    private ProduceManaComponent()
    {

    }
    public static Result<ProduceManaComponent> Create(ICard card) => Create(card.MainFace.OracleText);

    public static Result<ProduceManaComponent> Create(string oracleText)
    {
        if (string.IsNullOrWhiteSpace(oracleText))
            return Result<ProduceManaComponent>.Failure("Oracle text is empty.");

        var lines = oracleText.Split('\n');

        string? manaLine = lines.FirstOrDefault(l =>
            l.Contains("{T}", StringComparison.OrdinalIgnoreCase) &&
            l.Contains("Add", StringComparison.OrdinalIgnoreCase));

        if (manaLine == null)
            return Result<ProduceManaComponent>.Failure("No tap-for-mana ability found in oracle text.");

        Result<ManaUnit> rmu;
        ProduceManaComponent pmc = new();

        // Dynamic
        var dynamicResult = TryParseDynamicMana(manaLine);
        if (dynamicResult.IsSuccess)
        {
            rmu = ManaUnit.CreateDynamic(dynamicResult.Value);
            if (rmu.IsFailure) return rmu.ToFailure<ProduceManaComponent>();

            pmc._mana.Add(rmu.Value);
            return Result<ProduceManaComponent>.Success(pmc);
        }

        var match = GetAddManaLineRegex().Match(manaLine);
        if (!match.Success)
            return Result<ProduceManaComponent>.Failure("Could not parse tap ability line.");

        string capturedText = match.Groups[1].Value;
        var symbolMatches = GetManaSymbolRegex().Matches(capturedText);

        var parsedTypes = new List<ManaType>();
        foreach (var symbolMatch in symbolMatches.ToList())
        {
            string symbolCode = symbolMatch.Groups[1].Value.ToUpperInvariant();
            if (TryParseManaType(symbolCode, out var manaType))
                parsedTypes.Add(manaType);
        }

        if (parsedTypes.Count == 0)
            return Result<ProduceManaComponent>.Failure("No valid mana types parsed.");

        bool isChoicePattern = capturedText.Contains(" or ", StringComparison.OrdinalIgnoreCase);

        //Choice
        if (isChoicePattern)
        {
            rmu = ManaUnit.CreateChoice(parsedTypes);
            if (rmu.IsFailure) return rmu.ToFailure<ProduceManaComponent>();
            pmc._mana.Add(rmu.Value);
            return Result<ProduceManaComponent>.Success(pmc);
        }

        // Fixed
        foreach (var type in parsedTypes.ToList())
        {
            rmu = ManaUnit.CreateFixed(type);
            if (rmu.IsFailure) return rmu.ToFailure<ProduceManaComponent>();

            pmc._mana.Add(rmu.Value);
        }

        return Result<ProduceManaComponent>.Success(pmc);
    }

    private static Result<ManaDynamicType> TryParseDynamicMana(string manaLine)
    {
        if (manaLine.Contains("commander's color identity", StringComparison.OrdinalIgnoreCase))
            return Result<ManaDynamicType>.Success(ManaDynamicType.CommanderColorIdentity);

        if (manaLine.Contains("opponent controls could produce", StringComparison.OrdinalIgnoreCase) ||
            manaLine.Contains("opponents control", StringComparison.OrdinalIgnoreCase))
            return Result<ManaDynamicType>.Success(ManaDynamicType.OpponentLandColor);

        if (manaLine.Contains("any color", StringComparison.OrdinalIgnoreCase) ||
            manaLine.Contains("one mana of any", StringComparison.OrdinalIgnoreCase) ||
            manaLine.Contains("two mana of any", StringComparison.OrdinalIgnoreCase) ||
            manaLine.Contains("three mana of any", StringComparison.OrdinalIgnoreCase))
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