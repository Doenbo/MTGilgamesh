namespace MTG.Core;

public class Cheats
{
#pragma warning disable CA2211 // Non-constant fields should not be visible
    // Skips
    public static bool SkipUpkeepAndDraw = true;
    public static bool SkipCompleteCombatPhase = true;
    public static bool SkipEndStep = true;
    public static bool SkipPrio = true;

    // Infinites
    public static bool CanPlayInfiniteLands = true;
    public static bool CanPlaySpellsWithoutPaying = true;
    public static bool CanTapLandsInfiniteTimes = true;
    public static bool CanUseAbilitiesInfiniteTimes = true;
#pragma warning restore CA2211 // Non-constant fields should not be visible
}
