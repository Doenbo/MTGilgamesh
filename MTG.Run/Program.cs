using Microsoft.Extensions.Logging;
using MTG.Core.Cards;
using MTG.Core.Helper;
using MTG.Engine.Factories;
using MTG.Engine.Gameplay;
using MTG.Opponent;
using MTG.Resources.Enums;

namespace MTG.Run;

public class Program
{
    public static async Task Main()
    {
        IPlayerInputProvider human = new ConsoleInputProvider();
        IPlayerInputProvider ai = new OpponentInputProvider();

        var context = await GameContext.Create(human, ai);
        WriteAndExitIfFailure(context);

        IGameDisplay consoleDisplay = new ConsoleGameDisplay();

        var engine = new GameEngine(context.Value, consoleDisplay);

        engine.StartGameLoop();
    }

    private static async Task SomeTests()
    {
        var card1 = await CardCreator.GetExact("The Destined Warrior");
        WriteAndExitIfFailure(card1);

        var card2 = await CardCreator.GetExact(new CardRef()
        {
            Name = "Hildibrand Manderville // Gentleman's Rise",
            Set = "fic",
            CollectorNumber = "83"
        });
        WriteAndExitIfFailure(card2);
    }

    private static void WriteAndExitIfFailure<T>(Result<T> t)
    {
        if (t.IsFailure || t.Value == null)
        {
            Console.WriteLine(t.Error);
            System.Environment.Exit(1);
        }
        Console.WriteLine(t.Value.ToString());
    }
}