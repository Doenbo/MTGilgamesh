using MTG.Core.Helper;
using MTG.Core.Properties;

namespace MTG.Core.Components;

public class PlaneswalkerComponent : ICardComponent
{
    public Loyalty Loyalty { get; private set; }

    private PlaneswalkerComponent(Loyalty loyalty)
    {
        Loyalty = loyalty;
    }

    public static Result<PlaneswalkerComponent> Create(string loyalty)
    {
        return Result<PlaneswalkerComponent>.Success(new PlaneswalkerComponent(new Loyalty(loyalty)));
    }
}
