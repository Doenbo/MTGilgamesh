using MTG.Core.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace MTG.Engine.States;

public class CreatureState : ICardState
{
    private readonly CreatureComponent _template;

    public int PlusOneCounters { get; set; }
    public int PowerBuff { get; set; }
    public int ToughnessBuff { get; set; }

    public CreatureState(CreatureComponent template)
    {
        _template = template;
    }

    public int CurrentPower => (_template.Power ?? 0) + PlusOneCounters + PowerBuff;
    public int CurrentToughness => (_template.Toughness ?? 0) + PlusOneCounters + ToughnessBuff;
}
