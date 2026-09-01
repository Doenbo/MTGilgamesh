using Microsoft.EntityFrameworkCore;
using MTG.Core;
using MTG.Core.Enums;
using MTG.Core.Helper;
using MTG.Core.Types;
using MTG.Engine.Enums;
using MTG.Engine.Gameplay;

using ENV = System.Environment;
using CIS = MTG.Run.ConsoleInputStrings;

namespace MTG.Run;

public class ConsoleInputProvider : IPlayerInputProvider
{
    public async Task<PlayerAction> GetNextAction(GameContext context, CommanderPlayer player)
    {
        bool holdsStackPriority = context.StackCount > 0;

        if (context.TurnStep == TurnStep.Untap)
            return new PlayerAction(player, ActionType.PassPriority);

        if (holdsStackPriority)
            return GetCastSpellReaction(context, player);

        if (player == context.ActivePlayer && (context.TurnStep == TurnStep.Main1 || context.TurnStep == TurnStep.Main2))
            return GetMainStepAction(context, player);

        return GetPriorityAction(context, player);
    }

    private static PlayerAction GetMainStepAction(GameContext context, CommanderPlayer player)
    {
        if (AnyCheatSkipActive(context, player))
            return new PlayerAction(player, ActionType.PassPriority);

        Result<CardInstance> chosenCard;
        while (true)
        {
            context.Display.LogInfo(
                $"{ENV.NewLine}[{context.TurnStep}] {context.PriorityPlayer.Name}, it's your main phase. What do you do?" +
                $"{ENV.NewLine}1: Play a Card from your Hand | 2: Tap Land for Mana | 3: Activate Ability | " +
                $"{(Cheats.CanSeeOtherPlayersStuff ? CIS.f_ocheat + " | " : "")}" +
                $"{CIS.f_endph}");

            var input = Console.ReadLine(); //TODO? via other class?

            switch (input)
            {
                case "1":
                    chosenCard = ChooseHandCard(context, player);

                    if (chosenCard.IsFailure)
                        continue;

                    return new PlayerAction(player, ActionType.PlayCard, chosenCard.Value);

                case "2":
                    chosenCard = ChooseCardFromOwnBoard(context, player, CardType.Land);

                    if (chosenCard.IsFailure)
                        continue;

                    return new PlayerAction(player, ActionType.TapLandForMana, chosenCard.Value);

                case "3":
                    chosenCard = ChooseCardFromOwnBoard(context, player);

                    if (chosenCard.IsFailure)
                        continue;

                    return new PlayerAction(player, ActionType.PlayCard, chosenCard.Value);

                case "B":
                    context.Display.RenderBattlefield(context);
                    continue;

                case "M":
                    context.Display.RenderManaPool(context);
                    continue;

                case "S":
                    context.Display.RenderStack(context);
                    continue;

                case "OH":
                    if (Cheats.CanSeeOtherPlayersStuff)
                    { context.Display.RenderOpponentsHands(context); }
                    continue;

                case "OL":
                    if (Cheats.CanSeeOtherPlayersStuff)
                    { context.Display.RenderOpponentsLibraries(context); }
                    continue;

                case "0":
                    return new PlayerAction(player, ActionType.PassPriority);

                default:
                    context.Display.LogError("Could not process input. Try again!");
                    continue;
            }
        }
    }

    private static PlayerAction GetCastSpellReaction(GameContext context, CommanderPlayer player)
    {
        if (AnyCheatSkipActive(context, player))
            return new PlayerAction(player, ActionType.PassPriority);

        Result<CardInstance> chosenCard;
        while (true)
        {
            var topStackCard = context.PeekStack();
            string casterName = topStackCard.Owner.Name;

            context.Display.LogInfo(
                $"{ENV.NewLine}[{casterName}] has casted {topStackCard.CardData.FullName}." +
                $"[{player.Name}] How do you react?" +
                $"{ENV.NewLine}1: Play a Card from your Hand | {CIS.f_passp}");

            string? input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    chosenCard = ChooseHandCard(context, player);

                    if (chosenCard.IsFailure)
                        continue;

                    return new PlayerAction(player, ActionType.PlayCard, chosenCard.Value);

                case "2":
                    chosenCard = ChooseCardFromOwnBoard(context, player);

                    if (chosenCard.IsFailure)
                        continue;

                    return new PlayerAction(player, ActionType.PlayCard, chosenCard.Value);

                case "B":
                    context.Display.RenderBattlefield(context);
                    continue;

                case "M":
                    context.Display.RenderManaPool(context);
                    continue;

                case "S":
                    context.Display.RenderStack(context);
                    continue;

                case "0":
                    return new PlayerAction(player, ActionType.PassPriority);

                default:
                    context.Display.LogError("Could not process input. Try again!");
                    continue;
            }
        }
    }

    private static PlayerAction GetPriorityAction(GameContext context, CommanderPlayer player)
    {
        if (AnyCheatSkipActive(context, player))
            return new PlayerAction(player, ActionType.PassPriority);

        Result<CardInstance> chosenCard;
        while (true)
        {
            context.Display.LogInfo(
                $"{ENV.NewLine}[{context.TurnStep}] Priority: {player.Name}. What do you do?" +
                $"{ENV.NewLine}1: Play a Card from your Hand | 2: Tap a Card | {CIS.f_passp}");

            var input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    chosenCard = ChooseHandCard(context, player);

                    if (chosenCard.IsFailure)
                        continue;

                    return new PlayerAction(player, ActionType.PlayCard, chosenCard.Value);

                case "2":
                    chosenCard = ChooseCardFromOwnBoard(context, player);

                    if (chosenCard.IsFailure)
                        continue;

                    return new PlayerAction(player, ActionType.PlayCard, chosenCard.Value);

                case "B":
                    context.Display.RenderBattlefield(context);
                    continue;

                case "M":
                    context.Display.RenderManaPool(context);
                    continue;

                case "S":
                    context.Display.RenderStack(context);
                    continue;

                case "0":
                    return new PlayerAction(player, ActionType.PassPriority);

                default:
                    context.Display.LogError("Could not process input. Try again!");
                    continue;
            }
        }
    }

    private static Result<CardInstance> ChooseHandCard(GameContext context, CommanderPlayer player)
    {
        var text = $"\n{context.PriorityPlayer.Name}, which card would you like to play from your hand?\n";
        for (int i = 0; i < player.Hand.Count; i++)
        {
            var c = player.Hand[i];
            text += $"{i + 1}: {c.CardData.FullName} | ";
        }
        text += CIS.f_retur;

        while (true)
        {
            context.Display.LogInfo(text);

            var input = Console.ReadLine();

            if (string.IsNullOrEmpty(input))
                continue;

            if (input == "0")
                return Result<CardInstance>.Failure("Return!");

            if (CIS.stdCommands.Contains(input))
            {
                ExecMiscCommand(context, input);
                continue;
            }

            if (!int.TryParse(input, out int j) || j < 1 || j > player.Hand.Count + 1)
            {
                context.Display.LogError("Could not process input. Try again!");
                continue;
            }

            return Result<CardInstance>.Success(player.Hand[j - 1]);
        }
    }

    private static Result<CardInstance> ChooseCardFromOwnBoard(GameContext context, CommanderPlayer player, CardType filter = CardType.None)
    {
        var text = $"\n{context.PriorityPlayer.Name}, which card would you like to play from your board?\n";
        var playerBoard = context.GetBoardOf(player, filter);

        if (!playerBoard.Any() && filter == CardType.None)
            return Result<CardInstance>.Failure($"Your Board is empty!");

        if (!playerBoard.Any() && filter != CardType.None)
            return Result<CardInstance>.Failure($"No {filter} on your Board!");

        for (int i = 0; i < playerBoard.Count(); i++)
        {
            var c = playerBoard.ElementAt(i);
            text += $"{i + 1}: {c.CardData.FullName} | ";
        }
        text += CIS.f_retur;

        while (true)
        {
            context.Display.LogInfo(text);

            var input = Console.ReadLine();

            if (string.IsNullOrEmpty(input))
                continue;

            if (input == "0")
                return Result<CardInstance>.Failure("Return!");

            if (CIS.stdCommands.Contains(input))
            {
                ExecMiscCommand(context, input);
                continue;
            }

            if (!int.TryParse(input, out int j) || j < 1 || j > playerBoard.Count() + 1)
            {
                context.Display.LogError("Could not process input. Try again!");
                continue;
            }

            return Result<CardInstance>.Success(playerBoard.ElementAt(j - 1));
        }
    }

    private static void ExecMiscCommand(GameContext context, string input)
    {
        switch (input)
        {
            case "B":
                context.Display.RenderBattlefield(context);
                return;

            case "M":
                context.Display.RenderManaPool(context);
                return;

            case "S":
                context.Display.RenderStack(context);
                return;
        }
    }

    private static bool AnyCheatSkipActive(GameContext context, CommanderPlayer player)
    {
        if (Cheats.SkipUpkeepAndDraw &&
           (context.TurnStep == TurnStep.Upkeep || context.TurnStep == TurnStep.Draw))
            return true;

        if (Cheats.SkipCompleteCombatPhase &&
           (context.TurnStep == TurnStep.CombatBegin || context.TurnStep == TurnStep.DeclareAttackers ||
            context.TurnStep == TurnStep.DeclareBlockers || context.TurnStep == TurnStep.CombatDamage ||
            context.TurnStep == TurnStep.EndOfCombat))
            return true;

        if (Cheats.SkipEndStep &&
           (context.TurnStep == TurnStep.EndStep || context.TurnStep == TurnStep.CleanupStep))
            return true;

        if (Cheats.SkipPrio &&
           (context.StackCount > 0 || context.ActivePlayer != player))
            return true;

        return false;
    }
}
