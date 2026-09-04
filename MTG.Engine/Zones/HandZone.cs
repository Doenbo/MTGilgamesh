using MTG.Core.Enums;
using MTG.Engine.Cards;
using System;
using System.Collections.Generic;
using System.Text;

namespace MTG.Engine.Zones;

public class HandZone : BaseZone
{
    public override ZoneType Type => ZoneType.Hand;

    public override bool IsPublic => false;

    public int MaximumHandSize { get; set; } = 7;
}