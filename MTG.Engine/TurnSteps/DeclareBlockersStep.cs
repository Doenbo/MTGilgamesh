using MTG.Engine.Enums;

namespace MTG.Engine.TurnSteps;

public class DeclareBlockersStep : TurnStepBase
{
    public override TurnStep Name { get; init; } = TurnStep.DeclareBlockers;
}