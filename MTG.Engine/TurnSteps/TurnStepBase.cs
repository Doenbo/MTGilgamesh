using MTG.Core;
using MTG.Core.Components;
using MTG.Core.Components.OracleText;
using MTG.Core.Enums;
using MTG.Engine.Enums;
using MTG.Engine.Events;
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
        context.Display.LogGameEvent(new StepTransitionEvent(Name, context.ActivePlayer.Name));
        PerformTurnBasedActions(context);

        context.PriorityPlayer = context.ActivePlayer;
        context.ConsecutivePasses = 0;
    }

    public virtual void HandleAction(GameContext context, PlayerAction action)
    {
        switch (action.Type)
        {
            case ActionType.Concede:
                action.Player.IsEliminated = true;
                context.Display.LogInfo($"{action.Player.Name} has conceded.");
                context.RemovePlayerFromGame(action.Player);
                break;

            case ActionType.PassPriority:
                context.Display.LogInfo($"{action.Player.Name} passes priority.");
                context.PassPriority();
                break;

            case ActionType.TapLandForMana:
                HandleTapLandForMana(context, action);
                break;

            case ActionType.PlayCard:
                HandlePlayCard(context, action);
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
                context.Display.LogError($"Cannot play land {card.CardData.Name} right now (Requires main phase & empty stack).");
                return;
            }

            if (!Cheats.CanPlayInfiniteLands && context.HasPlayedLandThisTurn)
            {
                context.Display.LogError("You can only play one land each turn!");
                return;
            }

            context.HasPlayedLandThisTurn = true;
            player.RemoveFromHand(card);
            context.MoveToBattlefield(card);
            context.Display.LogInfo($"{player.Name} plays land: {card.CardData.Name}");
            context.OnPlayerTookAction();
            return;
        }

        // --- SPELL LOGIC ---
        bool isSorcerySpeed = !card.CardData.IsInstant() && !card.CardData.IsLand();
        if (isSorcerySpeed && (!CanPlaySorcerySpeed(context, player) || context.StackCount > 0))
        {
            context.Display.LogError($"Cannot cast {card.CardData.Name} right now (Sorcery timing rule).");
            return;
        }

        // --- ATTEMPT TO PAY MANA ---
        var mps = new ManaPayService();
        var res = card.CardData.MainFace.TryGetComponent<ManaCostComponent>(out var mcc);
        if (!res || mcc == null)
        {
            context.Display.LogError($"Cannot get Mana Cost!");
            return;
        }

        var payResult = mps.TryPay(mcc.ManaCost, player.ManaPool);
        if (payResult.IsFailure)
        {
            context.Display.LogError($"Cannot cast {card.CardData.Name}: {payResult.Error}");
            return;
        }

        player.UpdateManaPool(payResult.Value);

        // --- CAST SPELL AND START PRIORITY ---
        context.CastSpellAndStartPriorityRound(action);
        context.OnPlayerTookAction();
    }

    protected virtual void HandleTapLandForMana(GameContext context, PlayerAction action)
    {
        var card = action.TargetCardInstance;
        var player = action.Player;

        if (card == null || (card.IsTapped && !Cheats.CanTapLandsInfiniteTimes))
        {
            context.Display.LogError($"Cannot tap card for Mana!");
            return;
        }

        var result = card.CardData.MainFace.TryGetComponent<ProduceManaComponent>(out var pmc);

        if (!result || pmc == null)
        {
            context.Display.LogError($"Cannot get Produced Mana!");
            return;
        }

        foreach (var mana in pmc.ManaUnits.ToList())
        {
            if (pmc.RequiresTap) card.IsTapped = true;
            player.ManaPool.AddMana(mana);
        }
    }

    public virtual void OnStepExit(GameContext context)
    {
        // CR 500.4: Empty unspent mana pools at the end of each step/phase
        //context.ResetManaPools();
    }
}
