using Microsoft.SqlServer.Management.Smo;
using MTG.Engine.Enums;
using MTG.Engine.Gameplay;

namespace MTG.Opponent;

public class OpponentInputProvider : IPlayerInputProvider
{
    public async Task<PlayerAction> GetNextAction(GameContext context, CommanderPlayer player)
    {
        return new PlayerAction(player, ActionType.PassPriority);
    }
}
