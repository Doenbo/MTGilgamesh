using MTG.Core.Helper;

namespace MTG.Core.Components;

public class ManaComponent : ICardComponent
{
    public Mana ManaCost { get; init; }
    public float CMC { get; init; }

    private ManaComponent(Mana manacost, float cmc)
    {
        ManaCost = manacost;
        CMC = cmc;
    }

    public static Result<ManaComponent> Create(string manacost)
    {
        if (manacost == null)
            return Result<ManaComponent>.Failure($"ManaCost can't be null!");

        var mana = Mana.Create(manacost);
        if (mana.IsFailure)
            return mana.ToFailure<ManaComponent>();

        var cmc = mana.Value.GetCMC();
        if (cmc.IsFailure)
            return cmc.ToFailure<ManaComponent>();

        return Result<ManaComponent>.Success(new ManaComponent(mana.Value, cmc.Value));
    }
}
