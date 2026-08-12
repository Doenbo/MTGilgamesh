using System;
using System.Collections.Generic;
using System.Text;

namespace MTG.Core.Enums;

public enum ManaDynamicType
{
    None,
    CommanderColorIdentity,
    AnyColor,
    EachColor, //Bloom Tender
    OpponentLandColor,
    EachOpponentThatLostLife,
    NumberOfGates, //Baldur's Gate
    AmountOfLifeGained,
    EachCreature,
    EachCreatureWithDefender, //Axebane Guardian
    EachCreatureInParty, //Ardent Electromancer
    GreatestPower,
    GreatestToughness,
    ChargeCounter, //Altar of Shadows
}
