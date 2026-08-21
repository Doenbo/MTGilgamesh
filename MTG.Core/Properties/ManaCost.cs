using MTG.Core.Helper;
using MTG.Core.Properties;
using System.Text.RegularExpressions;

namespace MTG.Core;

public partial class ManaCost
{
    [GeneratedRegex(@"\{([WUBRGCXSP/\d]+)\}")]
    private static partial Regex CreateManaRegex();

    //Factory Pattern
    private string Value { get; set; }
    public List<ManaSymbol> Symbols { get; init; } = [];

    private ManaCost(string mana)
    {
        Value = mana;
    }

    public static Result<ManaCost> Create(string mana)
    {
        if (mana == null)
            return Result<ManaCost>.Failure("Null is not allowed for mana creation!");

        var result = new ManaCost(mana);

        //Empty Mana is allowed
        if (mana == string.Empty)
            return Result<ManaCost>.Success(result);

        //Split for Two-Faced Cards
        //var values = new List<ManaSymbol>();
        var faces = mana.Split(["//"], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        foreach (var face in faces.ToList())
        {
            var matches = CreateManaRegex().Matches(face);

            if (matches.Count == 0)
                return Result<ManaCost>.Failure("Not a convertible mana string!");

            foreach (var match in matches.ToList())
            {
                var bubbleContent = match.Groups[1].Value;

                var symbolResult = ManaSymbol.Create(bubbleContent);
                if (symbolResult.IsFailure)
                    return symbolResult.ToFailure<ManaCost>();

                result.Symbols.Add(symbolResult.Value);
            }
        }

        return Result<ManaCost>.Success(result);
    }

    public Result<float> GetCMC()
    {
        float count = 0;
        foreach (var symbol in Symbols.ToList())
        {
            var cmc = symbol.GetCMC();
            if (cmc.IsFailure)
                return cmc.ToFailure<float>();
            count += cmc.Value;
        }
        return Result<float>.Success(count);
    }

    public override bool Equals(object? obj)
    {
        if (obj is not ManaCost other) return false;

        // Vergleiche z. B. die Symbols-Liste oder den String-Value
        return this.ToString() == other.ToString();
    }

    public override int GetHashCode() => ToString().GetHashCode();

    public override string ToString() => Value;
}