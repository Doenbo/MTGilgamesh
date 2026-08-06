using MTG.Core.Cards;
using MTG.Core.Enums;
using MTG.Core.Helper;
using System;
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

    public bool RequiresTap { get; init; } = true;
    public IReadOnlyList<ManaType> FixedMana { get; init; }
    public IReadOnlyList<ManaType> ChoseMana { get; init; }
    public DynamicManaType DynamicMana { get; init; } = DynamicManaType.None;
    public bool IsFixed => FixedMana.Count > 0;
    public bool IsChoice => ChoseMana.Count > 0;
    public bool IsDynamic => DynamicMana != DynamicManaType.None;

    private ProduceManaComponent(List<ManaType> fixedMana, List<ManaType> manaChoices, DynamicManaType dynamicType, bool requiresTap)
    {
        FixedMana = fixedMana;
        ChoseMana = manaChoices;
        DynamicMana = dynamicType;
        RequiresTap = requiresTap;
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

        // 1. Commander's Color Identity
        if (manaLine.Contains("commander's color identity", StringComparison.OrdinalIgnoreCase))
            return Result<ProduceManaComponent>.Success(
                new ProduceManaComponent([], [], DynamicManaType.CommanderColorIdentity, true));

        // 2. Opponent Land
        if (manaLine.Contains("opponent controls could produce", StringComparison.OrdinalIgnoreCase) ||
            manaLine.Contains("opponents control", StringComparison.OrdinalIgnoreCase))
            return Result<ProduceManaComponent>.Success(
                new ProduceManaComponent([], [], DynamicManaType.OpponentLandColor, true));

        // 3. Any Color
        if (manaLine.Contains("any color", StringComparison.OrdinalIgnoreCase) ||
            manaLine.Contains("one mana of any type", StringComparison.OrdinalIgnoreCase))
            return Result<ProduceManaComponent>.Success(
                new ProduceManaComponent([], [], DynamicManaType.AnyColor, true));

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
            return Result<ProduceManaComponent>.Success(
                new ProduceManaComponent([], parsedTypes, DynamicManaType.None, true));

        return Result<ProduceManaComponent>.Success(
            new ProduceManaComponent(parsedTypes, [], DynamicManaType.None, true));
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