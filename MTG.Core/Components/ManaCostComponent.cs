using MTG.Core.Helper;

namespace MTG.Core.Components;

public class ManaCostComponent : ICardComponent
{
    public ManaCost ManaCost { get; init; }

    private ManaCostComponent(ManaCost manacost)
    {
        ManaCost = manacost;
    }

    public static Result<ManaCostComponent> Create(string? manacost)
    {
        if (manacost == null)
            return Result<ManaCostComponent>.Failure($"ManaCost can't be null!");

        var mana = ManaCost.Create(manacost);
        if (mana.IsFailure)
            return mana.ToFailure<ManaCostComponent>();

        return Result<ManaCostComponent>.Success(new ManaCostComponent(mana.Value));
    }

    public Result<float> GetCMC()
    {
        var cmc = ManaCost.GetCMC();
        return cmc.IsSuccess ? Result<float>.Success(cmc.Value) : cmc.ToFailure<float>();
    }
}
