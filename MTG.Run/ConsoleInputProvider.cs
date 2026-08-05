using MTG.Core.Cards;
using MTG.Core.Helper;
using MTG.Core.Types;
using MTG.Engine.Enums;
using MTG.Engine.Gameplay;
using System;
using System.CodeDom;

namespace MTG.Run;

public class ConsoleInputProvider : IPlayerInputProvider
{
    private const string s_board = "B: Show Own Board";
    private const string s_manap = "M: Show Own Mana Pool";
    private const string s_stack = "S: Show Stack";

    private const string o_passp = "0: Pass Priority";
    private const string o_endph = "0: End Phase";
    private const string o_retur = "0: Return";

    private const string f_passp = $"{s_board} | {s_manap} | {s_stack} | {o_passp}";
    private const string f_endph = $"{s_board} | {s_manap} | {o_endph}";
    private const string f_retur = $"{s_board} | {s_manap} | {o_retur}";

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
        Result<CardInstance> chosenCard;

        while (true)
        {
            Console.WriteLine($"\n[{context.TurnStep}] {context.PriorityPlayer.Name}, it's your main phase. What do you do?");
            Console.WriteLine($"1: Play a Card from your Hand | 2: Tap Land for Mana | 3: Activate Ability | {f_endph}");

            var input = Console.ReadLine();

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
                    Console.WriteLine($"{context.ToConsoleBattlefield()}");
                    continue;

                case "M":
                    Console.WriteLine($"{context.ToConsoleManaPool()}");
                    continue;

                case "0":
                    return new PlayerAction(player, ActionType.PassPriority);

                default:
                    Console.WriteLine("Could not process input. Try again!");
                    continue;
            }
        }
    }

    private static PlayerAction GetCastSpellReaction(GameContext context, CommanderPlayer player)
    {
        Result<CardInstance> chosenCard;

        while (true)
        {
            var topStackCard = context.PeekStack();
            string casterName = topStackCard.Owner.Name;

            Console.WriteLine($"\n[{casterName}] has casted {topStackCard.CardData.FullName}");
            Console.WriteLine($"[{player.Name}] How do you react?");
            Console.WriteLine($"1: Play a Card from your Hand | {f_passp}");

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
                    Console.WriteLine($"{context.ToConsoleBattlefield()}");
                    continue;

                case "M":
                    Console.WriteLine($"{context.ToConsoleManaPool()}");
                    continue;

                case "S":
                    Console.WriteLine($"{context.ToConsoleStack()}");
                    continue;

                case "0":
                    return new PlayerAction(player, ActionType.PassPriority);

                default:
                    Console.WriteLine("Could not process input. Try again!");
                    continue;
            }
        }
    }

    private static PlayerAction GetPriorityAction(GameContext context, CommanderPlayer player)
    {
        Result<CardInstance> chosenCard;

        while (true)
        {
            Console.WriteLine($"\n[{context.TurnStep}] Priority: {player.Name}. What do you do?");
            Console.WriteLine($"\n1: Play a Card from your Hand | 2: Tap a Card | {f_passp}");

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
                    Console.WriteLine($"{context.ToConsoleBattlefield()}");
                    continue;

                case "M":
                    Console.WriteLine($"{context.ToConsoleManaPool()}");
                    continue;

                case "S":
                    Console.WriteLine($"{context.ToConsoleStack()}");
                    continue;

                case "0":
                    return new PlayerAction(player, ActionType.PassPriority);

                default:
                    Console.WriteLine("Could not process input. Try again!");
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
        text += f_retur;

        while (true)
        {
            Console.WriteLine(text);

            var input = Console.ReadLine();
            if (!int.TryParse(input, out int j) || j < 1 || j > player.Hand.Count + 1)
            {
                Console.WriteLine("Could not process input. Try again!");
                continue;
            }

            if (j == 0)
                return Result<CardInstance>.Failure("Return!");

            return Result<CardInstance>.Success(player.Hand[j - 1]);
        }
    }

    private static Result<CardInstance> ChooseCardFromOwnBoard(GameContext context, CommanderPlayer player, CardType filter = CardType.None)
    {
        var text = $"\n{context.PriorityPlayer.Name}, which card would you like to play from your board?\n";
        var playerBoard = context.GetBoardOf(player, filter);

        if (playerBoard.Count() == 0 && filter == CardType.None)
            return Result<CardInstance>.Failure($"Your Board is empty!");

        if (playerBoard.Count() == 0 && filter != CardType.None)
            return Result<CardInstance>.Failure($"No {filter} on your Board!");

        for (int i = 0; i < playerBoard.Count(); i++)
        {
            var c = playerBoard.ElementAt(i);
            text += $"{i + 1}: {c.CardData.FullName} | ";
        }
        text += f_retur;

        while (true)
        {
            Console.WriteLine(text);

            var input = Console.ReadLine();
            if (!int.TryParse(input, out int j) || j < 1 || j > playerBoard.Count() + 1)
            {
                Console.WriteLine("Could not process input. Try again!");
                continue;
            }

            if (j == 0)
                return Result<CardInstance>.Failure("Return!");

            return Result<CardInstance>.Success(playerBoard.ElementAt(j - 1));
        }
    }
}
