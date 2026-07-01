using MTG.Engine.Enums;

namespace MTG.Engine.Gameplay;

public interface IGameDisplay
{
    void LogMessage(string message);
    void LogStepTransition(TurnStep name, string playerName);
    void LogElimination(string playerName);
    void RenderBattlefield(GameContext context);
}
