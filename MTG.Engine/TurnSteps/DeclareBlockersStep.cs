using MTG.Engine.Enums;
using MTG.Engine.Gameplay;

namespace MTG.Engine.TurnSteps;

public class DeclareBlockersStep : TurnStepBase
{
    public override TurnStep Name { get; init; } = TurnStep.DeclareBlockers;
}