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
        @"\{T\}:\s*Add\s+([^\.\n]+)",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private static readonly Regex AddManaRegex = new Regex(
        @"\{T\}:\s*Add\s+((?:\{[WUBRGC0-9]\})+)",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private static readonly Regex ManaSymbolRegex = new Regex(
        @"\{([WUBRGC0-9])\}",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private static readonly Regex AddChoiceManaRegex = new Regex(
    @"\{T\}:\s*Add\s+((?:\{[WUBRGC0-9]\}(?:,\s*|\s+or\s+|\s*)*)+)",
    RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public bool RequiresTap { get; private set; } = true;
    public IReadOnlyList<ManaType> FixedMana { get; init; }
    public IReadOnlyList<ManaType> ChoseMana { get; init; }
    public bool IsChoice => ChoseMana.Count > 0;

    private ProduceManaComponent(List<ManaType> fixedMana, List<ManaType> choseMana, bool requiresTap)
    {
        FixedMana = fixedMana;
        ChoseMana = choseMana;
        RequiresTap = requiresTap;
    }

    public static Result<ProduceManaComponent> Create(string oracleText)
    {
        if (string.IsNullOrWhiteSpace(oracleText))
            return Result<ProduceManaComponent>.Failure("Oracle text is empty.");

        // Split by lines to only evaluate the tap-for-mana line (ignores sacrifice/draw abilities)
        var lines = oracleText.Split('\n');
        string? manaLine = lines.FirstOrDefault(l => l.Contains("{T}: Add", StringComparison.OrdinalIgnoreCase));

        if (manaLine == null)
            return Result<ProduceManaComponent>.Failure("No tap-for-mana ability found in oracle text.");

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
            {
                parsedTypes.Add(manaType);
            }
        }

        if (parsedTypes.Count == 0)
            return Result<ProduceManaComponent>.Failure("No valid mana types parsed.");

        // Check if the text contains choice keywords like "or"
        bool isChoicePattern = capturedText.Contains(" or ", StringComparison.OrdinalIgnoreCase);

        if (isChoicePattern)
        {
            // Choices: Player picks ONE of the parsed types (e.g. [White, Black, Green])
            return Result<ProduceManaComponent>.Success(
                new ProduceManaComponent(new List<ManaType>(), parsedTypes, requiresTap: true));
        }

        // Fixed: Player gets ALL parsed types (e.g. Sol Ring -> [Colorless, Colorless])
        return Result<ProduceManaComponent>.Success(
            new ProduceManaComponent(parsedTypes, new List<ManaType>(), requiresTap: true));
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