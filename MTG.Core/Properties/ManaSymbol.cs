using MTG.Core.Helper;
using System.Collections.Immutable;

namespace MTG.Core.Properties;

public class ManaSymbol
{
    private static readonly ImmutableHashSet<char> ValidManaSymbols = [
        'W', 'U', 'B', 'R', 'G', 'C', 'X', 'S', 'P'
    ];

    //Factory Pattern
    private string Value { get; set; }

    private ManaSymbol(string value)
    {
        Value = value;
    }

    public static Result<ManaSymbol> Create(string value)
    {
        if (value == null)
            return Result<ManaSymbol>.Failure("Mana symbol cannot be null!");

        if (int.TryParse(value, out int numericValue) && numericValue >= 0)
        {
            return Result<ManaSymbol>.Success(new ManaSymbol(value));
        }

        var parts = value.Split('/');
        foreach (var part in parts)
        {
            if (int.TryParse(part, out int partNum) && partNum >= 0) continue;
            if (part.Length == 1 && ValidManaSymbols.Contains(part[0])) continue;

            return Result<ManaSymbol>.Failure($"Invalid component: {part}");
        }

        return Result<ManaSymbol>.Success(new ManaSymbol(value));
    }

    public Result<float> GetCMC()
    {
        if(Value == "X")
        {
            return Result<float>.Success(0);
        }
        else if (ValidManaSymbols.Contains(Value[0]))
        {
            return Result<float>.Success(1);
        }
        else if (int.TryParse(Value, out int numericValue) && numericValue >= 0)
        {
            return Result<float>.Success(numericValue);
        }
        else
            return Result<float>.Failure("Cannot calculate CMC!");
    }

    public override string ToString() => $"{{{Value}}}";
}

