using Microsoft.SqlServer.Management.Smo;
using MTG.Engine.Enums;
using MTG.Engine.Gameplay;

namespace MTG.Opponent;

public class OpponentInputProvider : IPlayerInputProvider
{
    public async Task<PlayerAction> GetNextAction(GameContext context, CommanderPlayer player)
    {
        if (context.ActivePlayer == player && context.TurnStep == TurnStep.Main1 && !context.HasPlayedLandThisTurn)
        {
            var c = player.Hand.FirstOrDefault(c => c.CardData.IsLand());
            if (c != null)
                return new PlayerAction(player, ActionType.PlayCard, c);
        }
        return new PlayerAction(player, ActionType.PassPriority);
    }
}
