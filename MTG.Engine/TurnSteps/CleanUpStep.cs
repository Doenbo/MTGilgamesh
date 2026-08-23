using MTG.Core.Enums;

namespace MTG.Engine.TurnSteps;

public class CleanUpStep : TurnStepBase
{
    public override TurnStep Name { get; init; } = TurnStep.CleanupStep;
}
