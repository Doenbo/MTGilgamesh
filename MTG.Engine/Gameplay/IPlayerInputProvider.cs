using System.Numerics;

namespace MTG.Engine.Gameplay;

public interface IPlayerInputProvider
{
    Task<PlayerAction> GetNextAction(GameContext context, CommanderPlayer player);
}
