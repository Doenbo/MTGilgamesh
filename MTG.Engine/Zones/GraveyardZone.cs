using MTG.Core.Enums;
using MTG.Engine.Cards;
using System;
using System.Collections.Generic;
using System.Text;

namespace MTG.Engine.Zones;

public class GraveyardZone : BaseZone
{
    public override ZoneType Type => ZoneType.Graveyard;

    public override bool IsPublic => true;
}