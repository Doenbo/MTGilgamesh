using MTG.Core.Cards;
using MTG.Engine.Enums;
using MTG.Engine.Gameplay;
using System;

namespace MTG.Run;

public class ConsoleInputProvider : IPlayerInputProvider
{
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
        while (true)
        {
            Console.WriteLine($"\n[{context.TurnStep}] {context.PriorityPlayer.Name}, it's your main phase. What do you do?");
            Console.WriteLine("1: Play a Card from your Hand | 2: Show Board | 3: Pass Priority (End Phase)");

            var input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    var input2 = ChooseHandCard(context, player);
                    if (!int.TryParse(input2, out int j) || j < 1 || j > player.Hand.Count + 1)
                    {
                        Console.WriteLine("Could not process input. Try again!");
                        continue;
                    }

                    if (j == player.Hand.Count + 1)
                        continue;

                    return new PlayerAction(player, ActionType.PlayCard, player.Hand[j - 1]);

                case "2":
                    Console.WriteLine($"{context.ToConsoleBattlefield()}");
                    continue;
                case "3":
                    return new PlayerAction(player, ActionType.PassPriority);
                default:
                    Console.WriteLine("Could not process input. Try again!");
                    continue;
            }
        }
    }

    private PlayerAction GetCastSpellReaction(GameContext context, CommanderPlayer player)
    {
        while (true)
        {
            var topStackCard = context.PeekStack();
            string casterName = topStackCard.Owner.Name;

            Console.WriteLine($"\n[{casterName}] has casted {topStackCard.CardData.FullName}");
            Console.WriteLine($"[{player.Name}] How do you react?");
            Console.WriteLine("1: Play a Card from your Hand | 2: Show Stack | 3: Pass Priority");

            string? input = Console.ReadLine();

            if (input == "1")
            {
                var input2 = ChooseHandCard(context, player);
                if (!int.TryParse(input2, out int j) || j < 1 || j > player.Hand.Count + 1)
                {
                    Console.WriteLine("Could not process input. Try again!");
                    continue;
                }

                if (j == player.Hand.Count + 1)
                    continue;

                return new PlayerAction(player, ActionType.PlayCard, player.Hand[j - 1]);
            }
            if (input == "2")
            {
                Console.WriteLine($"{context.ToConsoleStack()}");
                continue;
            }
            if (input == "3")
            {
                return new PlayerAction(player, ActionType.PassPriority);
            }

            Console.WriteLine("Could not process input. Try again!");
        }
    }

    private PlayerAction GetPriorityAction(GameContext context, CommanderPlayer player)
    {
        while (true)
        {
            Console.WriteLine($"\n[{context.TurnStep}] Priority: {player.Name}. What do you do?");
            Console.WriteLine("1: Play an Instant / Ability | 2: Show Board | 3: Pass Priority");

            var input = Console.ReadLine();

            if (input == "1")
            {
                var input2 = ChooseHandCard(context, player);
                if (!int.TryParse(input2, out int j) || j < 1 || j > player.Hand.Count + 1)
                {
                    Console.WriteLine("Could not process input. Try again!");
                    continue;
                }

                if (j == player.Hand.Count + 1)
                    continue;

                return new PlayerAction(player, ActionType.PlayCard, player.Hand[j - 1]);
            }
            if (input == "2")
            {
                Console.WriteLine($"{context.ToConsoleBattlefield()}");
                continue;
            }
            if (input == "3")
            {
                return new PlayerAction(player, ActionType.PassPriority);
            }

            Console.WriteLine("Could not process input. Try again!");
        }
    }

    private static string? ChooseHandCard(GameContext context, CommanderPlayer player)
    {
        Console.WriteLine($"\n{context.PriorityPlayer.Name}, which card would you like to play from your hand?\n");
        for (int i = 0; i < player.Hand.Count; i++)
        {
            var c = player.Hand[i];
            Console.Write($"{i + 1}: {c.CardData.FullName} | ");
        }
        Console.Write($"{player.Hand.Count + 1}: Return\n");

        return Console.ReadLine();
    }
}
