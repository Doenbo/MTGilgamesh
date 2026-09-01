using MTG.Core.Abilities;
using MTG.Core.Components;
using MTG.Core.Helper;

namespace MTG.Core.OracleTextParsers;

public interface ILineComponentParser
{
    bool CanParse(string line);

    Result<ICardComponent?> Parse(string line, CardContext cref);
}
