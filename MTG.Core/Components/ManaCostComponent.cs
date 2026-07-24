using MTG.Core.Helper;

namespace MTG.Core.Components;

public class ManaCostComponent : ICardComponent
{
    public ManaCost ManaCost { get; init; }
    public float CMC { get; init; }

    private ManaCostComponent(ManaCost manacost, float cmc)
    {
        ManaCost = manacost;
        CMC = cmc;
    }

    public static Result<ManaCostComponent> Create(string manacost)
    {
        if (manacost == null)
            return Result<ManaCostComponent>.Failure($"ManaCost can't be null!");

        var mana = ManaCost.Create(manacost);
        if (mana.IsFailure)
            return mana.ToFailure<ManaCostComponent>();

        var cmc = mana.Value.GetCMC();
        if (cmc.IsFailure)
            return cmc.ToFailure<ManaCostComponent>();

        return Result<ManaCostComponent>.Success(new ManaCostComponent(mana.Value, cmc.Value));
    }
}
