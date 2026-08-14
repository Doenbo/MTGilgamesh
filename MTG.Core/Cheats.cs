using System;
using System.Collections.Generic;
using System.Text;

namespace MTG.Core;

public class Cheats
{
    // Skips
    public const bool SkipUpkeepAndDraw = true;
    public const bool SkipCompleteCombatPhase = true;
    public const bool SkipEndStep = true;
    public const bool SkipPrio = true;

    // Infinites
    public const bool CanPlaySpellsWithoutPaying = true;
    public const bool CanTapLandsInfiniteTimes = true;
    public const bool CanUseAbilitiesInfiniteTimes = true;
}
