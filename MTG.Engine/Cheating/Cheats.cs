using System.Security.Policy;

namespace MTG.Engine.Cheating;

[Flags]
public enum Cheats
{
    //TODO More:
    //-Play Card From Graveyard / Exile
    //-Select Card From Library to Draw

    None = 0,

    // Skips
    SkipUpkeepAndDraw = 1 << 0, // 1
    SkipCompleteCombatPhase = 1 << 1, // 2
    SkipEndStep = 1 << 2, // 4
    SkipPrio = 1 << 3, // 8

    // Infinites
    CanPlayInfiniteLands = 1 << 4,          // 16
    CanPlaySpellsWithoutPaying = 1 << 5,    // 32
    CanTapLandsInfiniteTimes = 1 << 6,      // 64
    CanUseAbilitiesInfiniteTimes = 1 << 7,  // 128

    // Other Players
    CanSeeOtherPlayersStuff = 1 << 8, // 256

    // Presets
    All = Skips | Infinites | OtherPlayers,
    Skips = SkipUpkeepAndDraw | SkipCompleteCombatPhase | SkipEndStep | SkipPrio,
    Infinites = CanPlayInfiniteLands | CanPlaySpellsWithoutPaying | CanTapLandsInfiniteTimes | CanUseAbilitiesInfiniteTimes,
    OtherPlayers = CanSeeOtherPlayersStuff,
}