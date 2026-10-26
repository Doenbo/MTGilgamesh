using MTG.Core.Helper;

namespace MTG.Engine.Gameplay;

public interface IPlayerInputProvider
{
    Task<Result<PlayerAction>> GetNextAction(GameContext context, CommanderPlayer player);
}
