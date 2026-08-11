using System;
using System.Collections.Generic;
using System.Text;

namespace MTG.Core.Enums;

public enum ManaRestriction
{
    None,
    SpellsWithXInCost,
    CreatureSpellsOnly,
    CommanderOnly,
    InstantOrSorceryOnly
}
