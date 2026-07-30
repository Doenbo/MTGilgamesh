using MTG.Core.Enums;
using MTG.Core.Helper;
using System.Collections.Immutable;

namespace MTG.Core.Properties;

public class ManaSymbol
{
    private static readonly ImmutableHashSet<char> ValidManaSymbols = [
        'W', 'U', 'B', 'R', 'G', 'C', 'X', 'S', 'P'
    ];

    private string Value { get; init; }

    public List<ManaType> AcceptedColors { get; private set; } = [];

    public int GenericCost { get; private set; } = 0;
    public bool IsGenericOnly => AcceptedColors.Count == 0 && GenericCost > 0;

    //Factory Pattern
    private ManaSymbol(string value)
    {
        Value = value;
        ParseValue();
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

    private void ParseValue()
    {
        if (int.TryParse(Value, out int numeric))
        {
            GenericCost = numeric;
            return;
        }

        var parts = Value.Split('/');
        foreach (var part in parts)
        {
            if (int.TryParse(part, out int num))
            {
                GenericCost = num;
            }
            else if (part.Length == 1)
            {
                var color = CharToManaType(part[0]);
                if (color.HasValue)
                {
                    AcceptedColors.Add(color.Value);
                }
            }
        }
    }

    private static ManaType? CharToManaType(char c) => c switch
    {
        'W' => ManaType.White,
        'U' => ManaType.Blue,
        'B' => ManaType.Black,
        'R' => ManaType.Red,
        'G' => ManaType.Green,
        'C' => ManaType.Colorless,
        _ => null
    };

    public Result<float> GetCMC()
    {
        string[] parts = Value.Split('/');
        float maxCmc = -1f;

        foreach (string part in parts)
        {
            Result<float> partResult = GetSingleSymbolCmc(part);
            if (partResult.IsFailure)
            {
                return Result<float>.Failure("Cannot calculate CMC!");
            }

            maxCmc = Math.Max(maxCmc, partResult.Value);
        }

        return Result<float>.Success(maxCmc);
    }

    private Result<float> GetSingleSymbolCmc(string symbol)
    {
        if (symbol.Length == 1 && symbol[0] == 'X')
        {
            return Result<float>.Success(0);
        }
        if (symbol.Length == 1 && ValidManaSymbols.Contains(symbol[0]))
        {
            return Result<float>.Success(1);
        }
        if (int.TryParse(symbol, out int numericValue) && numericValue >= 0)
        {
            return Result<float>.Success(numericValue);
        }
        return Result<float>.Failure($"Cannot calculate CMC of pattern {this}!");
    }

    public override string ToString() => $"{{{Value}}}";
}

