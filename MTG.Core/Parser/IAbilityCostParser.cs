using MTG.Core.Abilities;
using MTG.Core.Helper;

namespace MTG.Core.Parser;

public interface IAbilityCostParser
{
    public Result<IReadOnlyList<IAbilityCost>> Parse(string rawCosts);
}
