using Microsoft.Extensions.Logging;
using MTG.Core.Cards;
using MTG.Core.Helper;
using MTG.Creator;
using MTG.Engine.Cheating;
using MTG.Engine.Gameplay;
using MTG.Opponent;

namespace MTG.Run;

public class Program
{
    public static async Task Main()
    {
        LogManager.Factory = LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Warning);
        });

        IPlayerInputProvider human = new ConsoleInputProvider();
        IPlayerInputProvider ai = new OpponentInputProvider();

        var context = await GameContext.Create(human, ai);
        WriteAndExitIfFailure(context);

        IGameDisplay consoleDisplay = new ConsoleGameDisplay();

        var engine = new GameEngine(context.Value, consoleDisplay);

        var end = await engine.StartGameLoop();
        if (end.IsFailure)
            Console.WriteLine(end.Error);
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