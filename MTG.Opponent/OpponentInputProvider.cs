using MTG.Core.Helper;
using MTG.Engine.Cheating;
using MTG.Engine.Enums;
using MTG.Engine.Gameplay;

namespace MTG.Opponent;

public class OpponentInputProvider : IPlayerInputProvider
{
    public async Task<Result<PlayerAction>> GetNextAction(GameContext context, CommanderPlayer player)
    {
        context.Display.IsLoggingErrors = false;

        foreach (var card in context.Battlefield.GetAllCardsFrom(player).Where(c => c.CardData.IsLand()))
        {
            context.HandleIncomingAction(new PlayerAction(player, ActionType.TapLandForMana, card));
        }

        foreach (var card in player.Hand.Cards.ToList())
        {
            context.HandleIncomingAction(new PlayerAction(player, ActionType.PlayCard, card));
        }

        context.Display.IsLoggingErrors = true;

        return Result<PlayerAction>.Success(new PlayerAction(player, ActionType.PassPriority));
    }
}