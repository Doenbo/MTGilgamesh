using MTG.Core.Enums;
using MTG.Engine.Cards;
using System;
using System.Collections.Generic;
using System.Text;

namespace MTG.Engine.Zones;

public class CommandZone : BaseZone
{
    public override ZoneType Type => ZoneType.Command;

    public override bool IsPublic => true;

    public void AddCommander(CardInstance commander)
    {
        _cards.Add(commander);
    }
}
