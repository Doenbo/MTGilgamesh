using MTG.Core.Abilities;
using MTG.Core.Helper;

namespace MTG.Core.Parser;

public interface IEffectParser
{
    public Result<IEffect> Parse(string rawEffect);
}
