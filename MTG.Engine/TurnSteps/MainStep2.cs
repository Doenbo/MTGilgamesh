using MTG.Engine.Enums;
using MTG.Engine.Gameplay;

namespace MTG.Engine.TurnSteps;

public class MainStep2 : ITurnStep
{
    public TurnStep Name { get; } = TurnStep.Main2;

    public void OnStepEnter(GameContext context)
    {
        context.Display.LogStepTransition(Name, context.ActivePlayer.Name);
        context.PriorityPlayer = context.ActivePlayer;
    }

    public void HandleAction(GameContext context, PlayerAction action)
    {
        switch (action.Type)
        {

            case ActionType.PassPriority:
                context.PassPriority();
                break;

            case ActionType.GoToNextPhase:
                context.TransitionTo(new EndStep());
                break;

            case ActionType.Concede:
                action.Player.LifeTotal = 0;
                context.Display.LogMessage($"{action.Player} has conceded");
                break;
        }
    }

    public void OnStepExit(GameContext context)
    {

    }
}
