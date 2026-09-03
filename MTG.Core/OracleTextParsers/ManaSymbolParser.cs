using MTG.Core.Enums;
using MTG.Core.Helper;
using System;
using System.Collections.Generic;
using System.Text;

namespace MTG.Core.OracleTextParsers;

public interface IManaSymbolParser
{
    Result<ManaType> ParseColorStrings(IEnumerable<string>? colorStrings);
}

public class ManaSymbolParser : IManaSymbolParser
{
    public Result<ManaType> ParseColorStrings(IEnumerable<string>? colorStrings)
    {
        if (colorStrings == null)
            return Result<ManaType>.Success(ManaType.None);

        var result = ManaType.None;

        foreach (var str in colorStrings)
        {
            switch (str.ToUpperInvariant())
            {
                case "W": result |= ManaType.White; break;
                case "U": result |= ManaType.Blue; break;
                case "B": result |= ManaType.Black; break;
                case "R": result |= ManaType.Red; break;
                case "G": result |= ManaType.Green; break;
                case "C": result |= ManaType.Colorless; break;
                default:
                    return Result<ManaType>.Failure($"Color '{str}' is invalid!");
            }
        }

        return Result<ManaType>.Success(result);
    }
}