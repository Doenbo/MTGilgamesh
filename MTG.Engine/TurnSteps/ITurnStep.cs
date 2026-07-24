using MTG.Engine.Enums;
using MTG.Engine.Gameplay;

namespace MTG.Engine.TurnSteps;

public interface ITurnStep
{
    public TurnStep Name { get; init; }
    public void OnStepEnter(GameContext gc);
    public void HandleAction(GameContext gc, PlayerAction action);
    public void OnStepExit(GameContext gc);
}
