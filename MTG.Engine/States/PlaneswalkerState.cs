using MTG.Core.Components;

namespace MTG.Engine.States;

public class PlaneswalkerState : ICardState
{
    public int CurrentLoyalty { get; set; }

    public PlaneswalkerState(PlaneswalkerComponent template)
    {
        CurrentLoyalty = template.Loyalty ?? 0;
    }
}