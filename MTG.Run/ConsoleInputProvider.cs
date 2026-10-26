using MTG.Core.Enums;
using MTG.Core.Helper;
using MTG.Core.Types;
using MTG.Engine.Cards;
using MTG.Engine.Cheating;
using MTG.Engine.Enums;
using MTG.Engine.Gameplay;
using System.Text;
using CIS = MTG.Run.ConsoleInputStrings;
using ENV = System.Environment;

namespace MTG.Run;

public class ConsoleInputProvider : IPlayerInputProvider
{
    public async Task<Result<PlayerAction>> GetNextAction(GameContext context, CommanderPlayer player)
    {
        bool holdsStackPriority = context.Stack.Count > 0;

        if (context.TurnStep == TurnStep.Untap)
            return Result<PlayerAction>.Success(new PlayerAction(player, ActionType.PassPriority));

        if (holdsStackPriority)
            return GetCastSpellReaction(context, player);

        if (player == context.ActivePlayer && (context.TurnStep == TurnStep.Main1 || context.TurnStep == TurnStep.Main2))
            return GetMainStepAction(context, player);

        return GetPriorityAction(context, player);
    }

    private static Result<PlayerAction> GetMainStepAction(GameContext context, CommanderPlayer player)
    {
        if (AnyCheatSkipActive(context, player))
            return Result<PlayerAction>.Success(new PlayerAction(player, ActionType.PassPriority));

        Result<CardInstance> chosenCard;
        while (true)
        {
            context.Display.LogInfo(
                $"[{context.TurnStep}] {context.PriorityPlayer.Name}, it's your main phase. What do you do?" +
                $"{ENV.NewLine}1: Play a Card from your Hand | 2: Tap Land for Mana | 3: Activate Ability | " +
                $"{(player.HasCheat(Cheats.CanSeeOtherPlayersStuff) ? CIS.s_cheat + " | " : "")}" +
                $"{CIS.f_endph}");

            var input = Console.ReadLine(); //TODO? via other class?

            if (TryHandleMiscCommand(context, input))
                continue;

            switch (input)
            {
                case "1":
                    chosenCard = ChooseHandCard(context, player);

                    if (chosenCard.IsFailure)
                        continue;

                    return Result<PlayerAction>.Success(new PlayerAction(player, ActionType.PlayCard, chosenCard.Value));

                case "2":
                    chosenCard = ChooseCardFromOwnBoard(context, player, CardType.Land);

                    if (chosenCard.IsFailure)
                    {
                        context.Display.LogInfo(chosenCard.Error);
                        continue;
                    }

                    return Result<PlayerAction>.Success(new PlayerAction(player, ActionType.TapLandForMana, chosenCard.Value));

                case "3":
                    chosenCard = ChooseCardFromOwnBoard(context, player);

                    if (chosenCard.IsFailure)
                        continue;

                    return Result<PlayerAction>.Success(new PlayerAction(player, ActionType.PlayCard, chosenCard.Value));

                case "C":
                    ChooseCheat(context, player);
                    continue;

                case "0":
                    return Result<PlayerAction>.Success(new PlayerAction(player, ActionType.PassPriority));

                default:
                    context.Display.LogError(CIS.s_error);
                    continue;
            }
        }
    }

    private static Result<PlayerAction> GetCastSpellReaction(GameContext context, CommanderPlayer player)
    {
        if (AnyCheatSkipActive(context, player))
            return Result<PlayerAction>.Success(new PlayerAction(player, ActionType.PassPriority));

        Result<CardInstance> chosenCard;
        while (true)
        {
            var topStackCard = context.PeekStack();

            if (topStackCard.IsFailure)
                return Result<PlayerAction>.Failure(topStackCard.Error);

            context.Display.LogInfo(
                $"{ENV.NewLine}[{topStackCard.Value.Controller.Name}] has casted {topStackCard.Value.Name}." +
                $"[{player.Name}] How do you react?" +
                $"{ENV.NewLine}1: Play a Card from your Hand | {CIS.f_passp}");

            var input = Console.ReadLine();

            if (TryHandleMiscCommand(context, input))
                continue;

            switch (input)
            {
                case "1":
                    chosenCard = ChooseHandCard(context, player);

                    if (chosenCard.IsFailure)
                        continue;

                    return Result<PlayerAction>.Success(new PlayerAction(player, ActionType.PlayCard, chosenCard.Value));

                case "2":
                    chosenCard = ChooseCardFromOwnBoard(context, player);

                    if (chosenCard.IsFailure)
                        continue;

                    return Result<PlayerAction>.Success(new PlayerAction(player, ActionType.PlayCard, chosenCard.Value));

                case "0":
                    return Result<PlayerAction>.Success(new PlayerAction(player, ActionType.PassPriority));

                default:
                    context.Display.LogError(CIS.s_error);
                    continue;
            }
        }
    }

    private static Result<PlayerAction> GetPriorityAction(GameContext context, CommanderPlayer player)
    {
        if (AnyCheatSkipActive(context, player))
            return Result<PlayerAction>.Success(new PlayerAction(player, ActionType.PassPriority));

        Result<CardInstance> chosenCard;
        while (true)
        {
            context.Display.LogInfo(
                $"{ENV.NewLine}[{context.TurnStep}] Priority: {player.Name}. What do you do?" +
                $"{ENV.NewLine}1: Play a Card from your Hand | 2: Tap a Card | {CIS.f_passp}");

            var input = Console.ReadLine();

            if (TryHandleMiscCommand(context, input))
                continue;

            switch (input)
            {
                case "1":
                    chosenCard = ChooseHandCard(context, player);

                    if (chosenCard.IsFailure)
                        continue;

                    return Result<PlayerAction>.Success(new PlayerAction(player, ActionType.PlayCard, chosenCard.Value));

                case "2":
                    chosenCard = ChooseCardFromOwnBoard(context, player);

                    if (chosenCard.IsFailure)
                        continue;

                    return Result<PlayerAction>.Success(new PlayerAction(player, ActionType.PlayCard, chosenCard.Value));

                case "0":
                    return Result<PlayerAction>.Success(new PlayerAction(player, ActionType.PassPriority));

                default:
                    context.Display.LogError(CIS.s_error);
                    continue;
            }
        }
    }

    private static Result<CardInstance> ChooseHandCard(GameContext context, CommanderPlayer player)
    {
        var sb = new StringBuilder();
        sb.Append($"\n{context.PriorityPlayer.Name}, which card would you like to play from your hand?\n");
        for (int i = 0; i < player.Hand.Count; i++)
        {
            var c = player.Hand.Cards[i];
            sb.Append($"{i + 1}: {c.CardData.MainFaceName} | ");
        }
        sb.Append(CIS.f_retur);

        while (true)
        {
            context.Display.LogInfo(sb.ToString());

            var input = Console.ReadLine();

            if (string.IsNullOrEmpty(input))
                continue;

            if (input == "0")
                return Result<CardInstance>.Failure("Return!");

            if (TryHandleMiscCommand(context, input))
                continue;

            if (!int.TryParse(input, out int j) || j < 1 || j > player.Hand.Count + 1)
            {
                context.Display.LogError(CIS.s_error);
                continue;
            }

            return Result<CardInstance>.Success(player.Hand.Cards[j - 1]);
        }
    }

    private static Result<CardInstance> ChooseCardFromOwnBoard(GameContext context, CommanderPlayer player, CardType filter = CardType.None)
    {
        var sb = new StringBuilder();
        sb.Append($"\n{context.PriorityPlayer.Name}, which card would you like to play from your board?\n");
        var playerBoard = context.GetBoardOf(player, filter);

        if (!playerBoard.Any() && filter == CardType.None)
            return Result<CardInstance>.Failure($"Your Board is empty!");

        if (!playerBoard.Any() && filter != CardType.None)
            return Result<CardInstance>.Failure($"No {filter} on your Board!");

        for (int i = 0; i < playerBoard.Count(); i++)
        {
            var c = playerBoard.ElementAt(i);
            sb.Append($"{i + 1}: {c.CardData.MainFaceName}");

            if (c.IsTapped)
                sb.Append("(T)");

            sb.Append(" | ");
        }

        if (playerBoard.Count() > 1)
            sb.Append("A: All | ");

        sb.Append(CIS.f_retur);

        while (true)
        {
            context.Display.LogInfo(sb.ToString());

            var input = Console.ReadLine();

            if (string.IsNullOrEmpty(input))
                continue;

            if (input == "0")
                return Result<CardInstance>.Failure("Return!");

            if (TryHandleMiscCommand(context, input))
                continue;

            if (!int.TryParse(input, out int j) || j < 1 || j > playerBoard.Count() + 1)
            {
                context.Display.LogError(CIS.s_error);
                continue;
            }

            return Result<CardInstance>.Success(playerBoard.ElementAt(j - 1));
        }
    }

    private static void ChooseCheat(GameContext context, CommanderPlayer player)
    {
        while (true)
        {
            context.Display.LogInfo(
                $"{ENV.NewLine}[{context.TurnStep}] {context.PriorityPlayer.Name}, it's your cheat phase. What do you do?" +
                $"{ENV.NewLine}{(player.HasCheat(Cheats.CanSeeOtherPlayersStuff) ? CIS.f_ocheat + " | " : "")}" +
                $"{CIS.o_endph}");

            string? input = Console.ReadLine();

            switch (input)
            {
                case "OH":
                    context.Display.RenderOpponentsHands(context);
                    return;

                case "OL":
                    context.Display.RenderOpponentsLibraries(context);
                    return;

                default:
                    context.Display.LogError(CIS.s_error);
                    continue;
            }
        }

    }

    private static bool TryHandleMiscCommand(GameContext context, string? input)
    {
        switch (input)
        {
            case "B":
                context.Display.RenderBattlefield(context);
                return true;

            case "M":
                context.Display.RenderManaPool(context);
                return true;

            case "S":
                context.Display.RenderStack(context);
                return true;

            default:
                return false;
        }
    }

    private static bool AnyCheatSkipActive(GameContext context, CommanderPlayer player)
    {
        if (player.HasCheat(Cheats.SkipUpkeepAndDraw) &&
           (context.TurnStep == TurnStep.Upkeep || context.TurnStep == TurnStep.Draw))
            return true;

        if (player.HasCheat(Cheats.SkipCompleteCombatPhase) &&
           (context.TurnStep == TurnStep.CombatBegin || context.TurnStep == TurnStep.DeclareAttackers ||
            context.TurnStep == TurnStep.DeclareBlockers || context.TurnStep == TurnStep.CombatDamage ||
            context.TurnStep == TurnStep.EndOfCombat))
            return true;

        if (player.HasCheat(Cheats.SkipEndStep) &&
           (context.TurnStep == TurnStep.EndStep || context.TurnStep == TurnStep.CleanupStep))
            return true;

        if (player.HasCheat(Cheats.SkipPrio) &&
           (context.Stack.Count > 0 || context.ActivePlayer != player))
            return true;

        return false;
    }
}