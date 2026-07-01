namespace MTG.Engine.Enums;

public enum TurnStep
{
    //Beginning Phase
    Untap,
    Upkeep,
    Draw,

    Main1,

    //Combat Phase
    CombatBegin,
    DeclareAttackers,
    DeclareBlockers,
    CombatDamage,
    EndOfCombat,

    Main2,

    //Ending Phase
    EndStep,
    CleanupStep
}
