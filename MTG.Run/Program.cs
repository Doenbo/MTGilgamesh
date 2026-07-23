using Microsoft.Extensions.Logging;
using MTG.Core.Cards;
using MTG.Core.Helper;
using MTG.Engine.Factories;
using MTG.Engine.Gameplay;
using MTG.Resources.Enums;

namespace MTG.Run;

public class Program
{
    public static async Task Main()
    {
        var context = await GameContext.Create();
        WriteAndExitIfFailure(context);

        IGameDisplay consoleDisplay = new ConsoleGameDisplay();
        IPlayerInputProvider consoleInput = new ConsoleInputProvider();

        var engine = new GameEngine(context.Value, consoleDisplay, consoleInput);

        engine.StartGameLoop();
    }

    private static async Task SomeTests(CommanderPlayer player)
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