using MTG.Engine.Enums;

namespace MTG.Engine.TurnSteps;

public class EndStep : TurnStepBase
{
    public override TurnStep Name { get; init; } = TurnStep.EndStep;
}
