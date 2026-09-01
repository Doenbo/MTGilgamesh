using MTG.Core.Abilities;
using MTG.Core.Helper;

namespace MTG.Core.OracleTextParsers;

public interface IEffectParser
{
    public Result<IEffect> Parse(string rawEffect);
}
