using MTG.Core.Helper;
using MTG.Core.Properties;

namespace MTG.Core.Components;

public class CreatureComponent : ICardComponent
{
    public Power Power { get; private set; }
    public Toughness Toughness { get; private set; }
    public int DamageTaken { get; private set; }

    private CreatureComponent(Power power, Toughness toughness)
    {
        Power = power;
        Toughness = toughness;
    }

    public static Result<CreatureComponent> Create(string? power, string? toughness)
    {
        return power == null || toughness == null
            ? Result<CreatureComponent>.Failure("Power and Toughness can't be null!")
            : Result<CreatureComponent>.Success(new CreatureComponent(new Power(power), new Toughness(toughness)));
    }
}
