using MTG.Engine.Enums;
using MTG.Engine.Gameplay;

namespace MTG.Engine.TurnSteps;

public class MainStep1 : ITurnStep
{
    public TurnStep Name { get; } = TurnStep.Main1;

    public void OnStepEnter(GameContext context)
    {
        context.Display.LogStepTransition(Name, context.ActivePlayer.Name);
        context.PriorityPlayer = context.ActivePlayer;
    }

    public void HandleAction(GameContext context, PlayerAction action)
    {
        switch (action.Type)
        {
            case ActionType.PlayCard:
                bool isReactionTime = context.StackCount > 0 || context.IsPhaseTransition;
                
                if (isReactionTime)
                    context.HandleReactionCast(action);
                else
                    HandleMainPhasePlay(context, action);
                break;

            case ActionType.PassPriority:
                context.Display.LogMessage($"{action.Player.Name} passes priority.");
                context.PassPriority();
                break;

            case ActionType.GoToNextPhase:
                if (context.StackCount > 0)
                {
                    context.Display.LogMessage("You cannot change phases while objects are on the stack!");
                    return;
                }

                context.IsPhaseTransition = true;
                context.PriorityRoundInitiator = action.Player;
                context.PassPriority();
                break;

            case ActionType.Concede:
                action.Player.LifeTotal = 0;
                context.Display.LogMessage($"{action.Player.Name} has conceded");
                break;
        }
    }

    public void HandleMainPhasePlay(GameContext context, PlayerAction action)
    {
        var card = action.TargetCardInstance;
        var player = action.Player;

        if (card.CardData.IsLand())
        {
            if (context.HasPlayedLandThisTurn)
            {
                context.Display.LogMessage("You can only play one Land each Turn!");
                return;
            }

            context.HasPlayedLandThisTurn = true;
            player.RemoveFromHand(card);
            context.MoveToBattlefield(card);
            context.Display.LogMessage($"{player.Name} plays Land: {card.CardData.FullName}");
            return;
        }
        else
        {
            context.CastSpellAndStartPriorityRound(action);
        }
    }

    public void OnStepExit(GameContext context)
    {
        //zB
        //context.ClearUnspentMana();
        //context.ResetStepFlags();
    }
}
