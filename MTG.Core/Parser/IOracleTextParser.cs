using MTG.Core.Abilities;
using MTG.Core.Components;
using MTG.Core.Helper;

namespace MTG.Core.Parser;

public interface IOracleTextParser
{
    Result<IReadOnlyList<ICardComponent>> Parse(string oracleText, CardContext cref);
}