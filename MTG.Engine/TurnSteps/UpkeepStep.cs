using MTG.Engine.Enums;

namespace MTG.Engine.TurnSteps;

public class UpkeepStep : TurnStepBase
{
    public override TurnStep Name { get; init; } = TurnStep.Upkeep;
}
