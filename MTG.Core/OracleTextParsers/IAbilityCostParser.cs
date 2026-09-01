using MTG.Core.Abilities;
using MTG.Core.Helper;

namespace MTG.Core.OracleTextParsers;

public interface IAbilityCostParser
{
    public Result<IReadOnlyList<IAbilityCost>> Parse(string rawCosts);
}
