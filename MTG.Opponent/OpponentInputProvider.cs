using MTG.Engine.Cheating;
using MTG.Engine.Enums;
using MTG.Engine.Gameplay;

namespace MTG.Opponent;

public class OpponentInputProvider : IPlayerInputProvider
{
    public async Task<PlayerAction> GetNextAction(GameContext context, CommanderPlayer player)
    {
        context.Display.IsLoggingErrors = false;

        foreach (var card in player.Hand.Cards.ToList())
        {
            //TODO
            context.HandleIncomingAction(new PlayerAction(player, ActionType.TapLandForMana, card));
            context.HandleIncomingAction(new PlayerAction(player, ActionType.PlayCard, card));
        }

        context.Display.IsLoggingErrors = true;

        return new PlayerAction(player, ActionType.PassPriority);
    }
}
