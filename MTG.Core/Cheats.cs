namespace MTG.Core;

public class Cheats
{
    // Skips
    public static bool SkipUpkeepAndDraw { get; set; } = false;
    public static bool SkipCompleteCombatPhase { get; set; } = false;
    public static bool SkipEndStep { get; set; } = false;
    public static bool SkipPrio { get; set; } = false;

    // Infinites
    public static bool CanPlayInfiniteLands { get; set; } = false;
    public static bool CanPlaySpellsWithoutPaying { get; set; } = false;
    public static bool CanTapLandsInfiniteTimes { get; set; } = false;
    public static bool CanUseAbilitiesInfiniteTimes { get; set; } = false;

    public static void EnableAll()
    {
        SkipUpkeepAndDraw = true;
        SkipCompleteCombatPhase = true;
        SkipEndStep = true;
        SkipPrio = true;
        CanPlayInfiniteLands = true;
        CanPlaySpellsWithoutPaying = true;
        CanTapLandsInfiniteTimes = true;
        CanUseAbilitiesInfiniteTimes = true;
    }

    public static void DisableAll()
    {
        SkipUpkeepAndDraw = false;
        SkipCompleteCombatPhase = false;
        SkipEndStep = false;
        SkipPrio = false;
        CanPlayInfiniteLands = false;
        CanPlaySpellsWithoutPaying = false;
        CanTapLandsInfiniteTimes = false;
        CanUseAbilitiesInfiniteTimes = false;
    }
}
