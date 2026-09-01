using MTG.Core.Abilities;
using MTG.Core.Components;
using MTG.Core.Helper;

namespace MTG.Core.OracleTextParsers;

public interface IOracleTextParser
{
    Result<IReadOnlyList<ICardComponent>> Parse(string oracleText, CardContext cref);
}