using MTG.Core.Enums;

namespace MTG.Engine.TurnSteps;

public class UpkeepStep : TurnStepBase
{
    public override TurnStep Name { get; init; } = TurnStep.Upkeep;
}
