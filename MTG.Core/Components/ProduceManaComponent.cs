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

public class ProduceManaComponent : ICardComponent
{
    private static readonly Regex AddManaLineRegex = new Regex(
        @"Add\s+([^\.\n]+)",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private static readonly Regex ManaSymbolRegex = new Regex(
        @"\{([WUBRGC0-9])\}",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);


    private List<ManaUnit> Mana { get; init; } = [];

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

        // 1. Commander's Color Identity
        if (manaLine.Contains("commander's color identity", StringComparison.OrdinalIgnoreCase))
        {
            rmu = ManaUnit.CreateDynamic(ManaDynamicType.CommanderColorIdentity);
            if (rmu.IsFailure) return rmu.ToFailure<ProduceManaComponent>();
            pmc.Mana.Add(rmu.Value);
            return Result<ProduceManaComponent>.Success(pmc);
        }

        // 2. Opponent Land
        if (manaLine.Contains("opponent controls could produce", StringComparison.OrdinalIgnoreCase) ||
            manaLine.Contains("opponents control", StringComparison.OrdinalIgnoreCase))
        {
            rmu = ManaUnit.CreateDynamic(ManaDynamicType.OpponentLandColor);
            if (rmu.IsFailure) return rmu.ToFailure<ProduceManaComponent>();
            pmc.Mana.Add(rmu.Value);
            return Result<ProduceManaComponent>.Success(pmc);
        }

        // 3. Any Color
        if (manaLine.Contains("any color", StringComparison.OrdinalIgnoreCase) ||
            manaLine.Contains("one mana of any", StringComparison.OrdinalIgnoreCase) ||
            manaLine.Contains("two mana of any", StringComparison.OrdinalIgnoreCase) ||
            manaLine.Contains("three mana of any", StringComparison.OrdinalIgnoreCase))
        {
            rmu = ManaUnit.CreateDynamic(ManaDynamicType.AnyColor);
            if (rmu.IsFailure) return rmu.ToFailure<ProduceManaComponent>();
            pmc.Mana.Add(rmu.Value);
            return Result<ProduceManaComponent>.Success(pmc);
        }

        // 4. Static Symbol Parsing ({W}, {U}, {B}, {R}, {G}, {C})
        var match = AddManaLineRegex.Match(manaLine);
        if (!match.Success)
            return Result<ProduceManaComponent>.Failure("Could not parse tap ability line.");

        string capturedText = match.Groups[1].Value;
        var symbolMatches = ManaSymbolRegex.Matches(capturedText);

        var parsedTypes = new List<ManaType>();
        foreach (Match symbolMatch in symbolMatches)
        {
            string symbolCode = symbolMatch.Groups[1].Value.ToUpperInvariant();
            if (TryParseManaType(symbolCode, out var manaType))
                parsedTypes.Add(manaType);
        }

        if (parsedTypes.Count == 0)
            return Result<ProduceManaComponent>.Failure("No valid mana types parsed.");

        bool isChoicePattern = capturedText.Contains(" or ", StringComparison.OrdinalIgnoreCase);

        if (isChoicePattern)
        {
            rmu = ManaUnit.CreateChoice(new List<ManaType>() { ManaType.Black }); //TODO
            if (rmu.IsFailure) return rmu.ToFailure<ProduceManaComponent>();
            pmc.Mana.Add(rmu.Value);
            return Result<ProduceManaComponent>.Success(pmc);
        }

        rmu = ManaUnit.CreateFixed(ManaType.Black); //TODO
        if (rmu.IsFailure) return rmu.ToFailure<ProduceManaComponent>();
        pmc.Mana.Add(rmu.Value);
        return Result<ProduceManaComponent>.Success(pmc);
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