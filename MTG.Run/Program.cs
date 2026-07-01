using MTG.Core.Cards;
using MTG.Core.Helper;
using MTG.Resources.Enums;
using MTG.Engine.Factories;
using MTG.Engine.Gameplay;

namespace MTG.Run;

public class Program
{
    public static async Task Main()
    {
        var context = GameContext.Create().Value;

        var p1 = await CommanderPlayer.Create("Dön", 40, CommanderPrecon.TokenTriumph);
        WriteLinePlayer(p1);
        context.AddPlayer(p1.Value);

        var p2 = await CommanderPlayer.Create("Nüs", 40, CommanderPrecon.ScionsAndSpellcraft);
        WriteLinePlayer(p2);
        context.AddPlayer(p2.Value);

        //IGameDisplay consoleDisplay = new ConsoleGameDisplay();
        //IPlayerInputProvider consoleInput = new ConsoleInputProvider();

        //var engine = new GameEngine(context, consoleDisplay, consoleInput);

        //engine.StartGameLoop();
    }

    private static async Task SomeTests(CommanderPlayer player)
    {
        var card1 = await CardCreator.GetExact("The Destined Warrior");
        WriteLineCard(card1);

        var card2 = await CardCreator.GetExact(new CardRef()
        {
            Name = "Hildibrand Manderville // Gentleman's Rise",
            Set = "fic",
            CollectorNumber = "83"
        });
        WriteLineCard(card2);
    }

    private static void WriteLineCard(Result<ICard> c)
    {
        if (c.IsFailure)
        {
            Console.WriteLine(c.Error);
            System.Environment.Exit(1);
        }
        else
            Console.WriteLine(c.Value.ToStringConsole());
    }

    private static void WriteLinePlayer(Result<CommanderPlayer> p)
    {
        if (p.IsFailure)
        {
            Console.WriteLine(p.Error);
            System.Environment.Exit(1);
        }
    }
}