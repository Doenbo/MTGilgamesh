using MTG.Core;
using MTG.Engine.Enums;
using MTG.Engine.Gameplay;

namespace MTG.Opponent;

public class OpponentInputProvider : IPlayerInputProvider
{
    public async Task<PlayerAction> GetNextAction(GameContext context, CommanderPlayer player)
    {
        Cheats.DisableAll();
        context.Display.IsLoggingErrors = false;

        foreach (var card in player.Hand.Cards.ToList())
        {
            context.HandleIncomingAction(new PlayerAction(player, ActionType.PlayCard, card));
        }

        Cheats.EnableAll();
        context.Display.IsLoggingErrors = true;

        return new PlayerAction(player, ActionType.PassPriority);
    }
}
