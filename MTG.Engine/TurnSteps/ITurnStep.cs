using MTG.Engine.Enums;
using MTG.Engine.Gameplay;

namespace MTG.Engine.TurnSteps;

public interface ITurnStep
{
    TurnStep Name { get; }
    void OnStepEnter(GameContext gc);
    void HandleAction(GameContext gc, PlayerAction action);
    void OnStepExit(GameContext gc);
}
