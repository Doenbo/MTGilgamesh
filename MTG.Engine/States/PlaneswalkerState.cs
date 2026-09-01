using MTG.Core.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace MTG.Engine.States;

public class PlaneswalkerState : ICardState
{
    public int CurrentLoyalty { get; set; }

    public PlaneswalkerState(PlaneswalkerComponent template)
    {
        CurrentLoyalty = template.Loyalty ?? 0;
    }
}