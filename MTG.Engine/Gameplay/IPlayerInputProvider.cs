using System.Numerics;

namespace MTG.Engine.Gameplay;

public interface IPlayerInputProvider
{
    PlayerAction GetNextAction(GameContext context, CommanderPlayer player);
}
