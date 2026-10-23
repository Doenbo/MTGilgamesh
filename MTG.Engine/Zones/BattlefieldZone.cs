using MTG.Core.Enums;
using MTG.Engine.Cards;
using MTG.Engine.Gameplay;
using System;
using System.Collections.Generic;
using System.Text;

namespace MTG.Engine.Zones;

public class BattlefieldZone : BaseZone
{
    public override ZoneType Type => ZoneType.Battlefield;

    public override bool IsPublic => true;

    public IReadOnlyList<CardInstance> GetAllCardsFrom(CommanderPlayer player)
    {
        return Cards.Where(c => c.Owner == player).ToList();
    }
}
