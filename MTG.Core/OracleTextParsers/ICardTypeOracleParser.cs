using MTG.Core.Abilities;
using MTG.Core.Components;
using MTG.Core.Helper;

namespace MTG.Core.OracleTextParsers;

public interface ICardTypeOracleParser
{
    bool CanHandle(CardContext context);
    Result<IReadOnlyList<ICardComponent>> Parse(string oracleText, CardContext context);
}
