using MTG.Core.Helper;
using MTG.Core.Properties;

namespace MTG.Core.Components;

public class BattleComponent : ICardComponent
{
    public Defense Defense { get; private set; }

    private BattleComponent(Defense defense)
    {
        Defense = defense;
    }

    public static Result<BattleComponent> Create(string defense)
    {
        return Result<BattleComponent>.Success(new BattleComponent(new Defense(defense)));
    }
}
