using MTG.Core.Cards;
using MTG.Engine.Enums;
using MTG.Engine.Gameplay;
using Newtonsoft.Json.Linq;
using System.Diagnostics.Eventing.Reader;

namespace MTG.Run;

public class ConsoleInputProvider //: IPlayerInputProvider
{
    public PlayerAction GetNextAction(GameContext context, CommanderPlayer player)
    {
        bool holdsStackPriority = context.StackCount > 0;
        bool isPhaseTransition = context.IsEndingTheStep;

        if (context.TurnStep == TurnStep.Untap ||
            context.TurnStep == TurnStep.Upkeep ||
            context.TurnStep == TurnStep.Draw ||
            context.TurnStep == TurnStep.EndStep ||
            context.TurnStep == TurnStep.CleanupStep)
        {
            return new PlayerAction(player, ActionType.GoToNextPhase);
        }

        if (context.TurnStep == TurnStep.CombatBegin ||
            context.TurnStep == TurnStep.DeclareAttackers ||
            context.TurnStep == TurnStep.DeclareBlockers ||
            context.TurnStep == TurnStep.CombatDamage ||
            context.TurnStep == TurnStep.EndOfCombat)
        {
            return new PlayerAction(player, ActionType.GoToNextPhase);
        }

        if (holdsStackPriority) return GetCastSpellReaction(context, player);
        if (isPhaseTransition) return GetPhaseTransitionReaction(context, player);
        if (player == context.ActivePlayer) return GetMainStepAction(context, player);

        throw new Exception("Unexpected game state.");
    }

    private PlayerAction GetMainStepAction(GameContext context, CommanderPlayer player)
    {
        while (true)
        {
            Console.WriteLine($"\n[{context.TurnStep}] {context.PriorityPlayer.Name}, it's your turn. What do you do?");
            Console.WriteLine("1: Play a Card from your Hand | 2: Show Board | 3: Go to next Phase");

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
                    return new PlayerAction(player, ActionType.GoToNextPhase);
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
            Console.WriteLine($"\n[{context.PriorityRoundInitiator.Name}] has casted {context.PeekStack().CardData.FullName}");
            Console.WriteLine($"[{player.Name}] How do you react?");
            Console.WriteLine("1: Play a Card from your Hand | 2: Show Stack | 3: Do not react");

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

    private PlayerAction GetPhaseTransitionReaction(GameContext context, CommanderPlayer player)
    {
        while (true)
        {
            Console.WriteLine($"\n[REACTION] {context.ActivePlayer.Name} wants to end '{context.TurnStep}'.");
            Console.WriteLine($"{player.Name}, how would you like to react?");
            Console.WriteLine("1: Activate Instant / Ability | 2: Pass");

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
