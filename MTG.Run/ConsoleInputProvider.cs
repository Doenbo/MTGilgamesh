using MTG.Core.Cards;
using MTG.Core.Helper;
using MTG.Engine.Enums;
using MTG.Engine.Gameplay;
using System;
using System.CodeDom;

namespace MTG.Run;

public class ConsoleInputProvider : IPlayerInputProvider
{
    private const string s_passp = "0: Pass Priority";
    private const string s_endph = "0: End Phase";
    private const string s_retur = "0: Return";

    public async Task<PlayerAction> GetNextAction(GameContext context, CommanderPlayer player)
    {
        bool holdsStackPriority = context.StackCount > 0;

        if (context.TurnStep == TurnStep.Untap)
        {
            return new PlayerAction(player, ActionType.PassPriority);
        }

        if (holdsStackPriority) return GetCastSpellReaction(context, player);
        if (player == context.ActivePlayer && (context.TurnStep == TurnStep.Main1 || context.TurnStep == TurnStep.Main2))
        {
            return GetMainStepAction(context, player);
        }

        return GetPriorityAction(context, player);
    }

    private PlayerAction GetMainStepAction(GameContext context, CommanderPlayer player)
    {
        Result<CardInstance> chosenCard;

        while (true)
        {
            Console.WriteLine($"\n[{context.TurnStep}] {context.PriorityPlayer.Name}, it's your main phase. What do you do?");
            Console.WriteLine("1: Play a Card from your Hand | 2: Tap a Card | 9: Show Board | " + s_endph);

            var input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    chosenCard = ChooseHandCard(context, player);

                    if (chosenCard.IsFailure)
                        continue;

                    return new PlayerAction(player, ActionType.PlayCard, chosenCard.Value);

                case "2":
                    chosenCard = ChooseOwnBoardCard(context, player);

                    if (chosenCard.IsFailure)
                        continue;

                    return new PlayerAction(player, ActionType.PlayCard, chosenCard.Value);

                case "9":
                    Console.WriteLine($"{context.ToConsoleBattlefield()}");
                    continue;

                case "0":
                    return new PlayerAction(player, ActionType.PassPriority);

                default:
                    Console.WriteLine("Could not process input. Try again!");
                    continue;
            }
        }
    }

    private PlayerAction GetCastSpellReaction(GameContext context, CommanderPlayer player)
    {
        Result<CardInstance> chosenCard;

        while (true)
        {
            var topStackCard = context.PeekStack();
            string casterName = topStackCard.Owner.Name;

            Console.WriteLine($"\n[{casterName}] has casted {topStackCard.CardData.FullName}");
            Console.WriteLine($"[{player.Name}] How do you react?");
            Console.WriteLine("1: Play a Card from your Hand | 9: Show Stack | " + s_passp);

            string? input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    chosenCard = ChooseHandCard(context, player);

                    if (chosenCard.IsFailure)
                        continue;

                    return new PlayerAction(player, ActionType.PlayCard, chosenCard.Value);

                case "2":
                    chosenCard = ChooseOwnBoardCard(context, player);

                    if (chosenCard.IsFailure)
                        continue;

                    return new PlayerAction(player, ActionType.PlayCard, chosenCard.Value);

                case "9":
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

    private PlayerAction GetPriorityAction(GameContext context, CommanderPlayer player)
    {
        Result<CardInstance> chosenCard;

        while (true)
        {
            Console.WriteLine($"\n[{context.TurnStep}] Priority: {player.Name}. What do you do?");
            Console.WriteLine("\"1: Play a Card from your Hand | 2: Tap a Card | 9: Show Board | " + s_passp);

            var input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    chosenCard = ChooseHandCard(context, player);

                    if (chosenCard.IsFailure)
                        continue;

                    return new PlayerAction(player, ActionType.PlayCard, chosenCard.Value);

                case "2":
                    chosenCard = ChooseOwnBoardCard(context, player);

                    if (chosenCard.IsFailure)
                        continue;

                    return new PlayerAction(player, ActionType.PlayCard, chosenCard.Value);

                case "9":
                    Console.WriteLine($"{context.ToConsoleBattlefield()}");
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
        text += s_retur;

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

    private static Result<CardInstance> ChooseOwnBoardCard(GameContext context, CommanderPlayer player)
    {
        var text = $"\n{context.PriorityPlayer.Name}, which card would you like to play from your board?\n";
        var playerBoard = context.GetBoardOf(player);
        for (int i = 0; i < playerBoard.Count(); i++)
        {
            var c = playerBoard.ElementAt(i);
            text += $"{i + 1}: {c.CardData.FullName} | ";
        }
        text += s_retur;

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
