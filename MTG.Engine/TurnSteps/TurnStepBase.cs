using MTG.Core.Components;
using MTG.Engine.Enums;
using MTG.Engine.Gameplay;
using MTG.Engine.Services;

namespace MTG.Engine.TurnSteps;

public abstract class TurnStepBase : ITurnStep
{
    public abstract TurnStep Name { get; init; }

    protected virtual void PerformTurnBasedActions(GameContext context) { }

    public virtual bool CanPlaySorcerySpeed(GameContext context, CommanderPlayer player)
    {
        return false;
    }

    public virtual void OnStepEnter(GameContext context)
    {
        context.Display?.LogStepTransition(Name, context.ActivePlayer.Name);
        PerformTurnBasedActions(context);

        context.PriorityPlayer = context.ActivePlayer;
        context.ConsecutivePasses = 0;
    }

    public virtual void HandleAction(GameContext context, PlayerAction action)
    {
        switch (action.Type)
        {
            case ActionType.PassPriority:
                context.Display?.LogMessage($"{action.Player.Name} passes priority.");
                context.PassPriority();
                break;

            case ActionType.PlayCard:
                HandlePlayCard(context, action);
                break;

            case ActionType.Concede:
                action.Player.IsEliminated = true;
                context.Display?.LogMessage($"{action.Player.Name} has conceded.");
                context.RemovePlayerFromGame(action.Player);
                break;
        }
    }

    protected virtual void HandlePlayCard(GameContext context, PlayerAction action)
    {
        var card = action.TargetCardInstance;
        var player = action.Player;

        if (card == null) return;

        // --- LAND LOGIC ---
        if (card.CardData.IsLand())
        {
            if (!CanPlaySorcerySpeed(context, player) || context.StackCount > 0)
            {
                context.Display?.LogMessage($"Cannot play land {card.CardData.FullName} right now (Requires main phase & empty stack).");
                return;
            }

            if (context.HasPlayedLandThisTurn)
            {
                context.Display?.LogMessage("You can only play one land each turn!");
                return;
            }

            context.HasPlayedLandThisTurn = true;
            player.RemoveFromHand(card);
            context.MoveToBattlefield(card);
            context.Display?.LogMessage($"{player.Name} plays land: {card.CardData.FullName}");
            context.OnPlayerTookAction();
            return;
        }

        // --- SPELL LOGIC ---
        bool isSorcerySpeed = !card.CardData.IsInstant() && !card.CardData.IsLand();
        if (isSorcerySpeed && (!CanPlaySorcerySpeed(context, player) || context.StackCount > 0))
        {
            context.Display?.LogMessage($"Cannot cast {card.CardData.FullName} right now (Sorcery timing rule).");
            return;
        }

        // --- ATTEMPT TO PAY MANA ---
        var mps = new ManaPayService();
        var res = card.CardData.MainFace.TryGetComponent<ManaCostComponent>(out var mc);
        if (!res)
        {
            context.Display?.LogMessage($"Cannot get Mana Cost!");
            return;
        }

        var payResult = mps.TryPay(mc.ManaCost, player.ManaPool);

        if (payResult.IsFailure)
        {
            context.Display?.LogMessage($"Cannot cast {card.CardData.FullName}: {payResult.Error}");
            return;
        }

        // --- UPDATE MANA POOL ---
        player.ManaPool = payResult.Value; //TODO is this override ok?

        // --- CAST SPELL AND START PRIORITY ---
        context.CastSpellAndStartPriorityRound(action);
        context.OnPlayerTookAction();
    }

    public virtual void OnStepExit(GameContext context)
    {
        // CR 500.4: Empty unspent mana pools at the end of each step/phase
    }
}
