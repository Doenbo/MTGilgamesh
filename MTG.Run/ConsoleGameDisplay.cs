using MTG.Core.Cards;
using MTG.Engine.Enums;
using MTG.Engine.Events;
using MTG.Engine.Gameplay;

namespace MTG.Run;

public class ConsoleGameDisplay : IGameDisplay
{
    public bool IsLoggingErrors { get; set; } = true;

    private void WriteColoredLine(string message, ConsoleColor cc)
    {
        Console.ForegroundColor = cc;
        Console.WriteLine(message);
        Console.ResetColor();
    }

    public void LogInfo(string message)
    {
        WriteColoredLine(message, ConsoleColor.White);
    }

    public void LogError(string message)
    {
        if (IsLoggingErrors)
            WriteColoredLine(message, ConsoleColor.Red);
    }

    public void LogGameEvent(IGameEvent gameEvent)
    {
        switch (gameEvent)
        {
            case StepTransitionEvent step:
                WriteColoredLine($"\n=== {step.Description} ===", ConsoleColor.Yellow);
                break;

            case SpellCastEvent spell:
                WriteColoredLine($"[CAST] {spell.PlayerName} played {spell.SpellName}", ConsoleColor.Cyan);
                break;

            case DamageDealtEvent damage:
                WriteColoredLine($"[DAMAGE] {damage.Source} -> {damage.Target}: {damage.Amount}", ConsoleColor.DarkRed);
                break;

            default:
                WriteColoredLine(gameEvent.Description, ConsoleColor.Gray);
                break;
        }
    }

    public void RenderBattlefield(GameContext context)
    {
        WriteColoredLine($"{context.ToConsoleBattlefield()}", ConsoleColor.White);
    }

    public void RenderManaPool(GameContext context)
    {
        WriteColoredLine($"{context.ToConsoleManaPool()}", ConsoleColor.White);
    }

    public void RenderStack(GameContext context)
    {
        WriteColoredLine($"{context.ToConsoleStack()}", ConsoleColor.Gray);
    }

    public void RenderOpponentsHands(GameContext context)
    {
        WriteColoredLine($"{context.ToConsoleOpponentsHands()}", ConsoleColor.White);
    }

    public void RenderOpponentsLibraries(GameContext context)
    {
        WriteColoredLine($"{context.ToConsoleOpponentsLibraries()}", ConsoleColor.White);
    }

    public void OnCardPlayed(CommanderPlayer player, ICard card)
    {
        WriteColoredLine("TODO", ConsoleColor.White);
    }

    public void OnCardTapped(ICard card, bool isTapped)
    {
        WriteColoredLine("TODO", ConsoleColor.White);
    }

    public void OnLifeTotalChanged(CommanderPlayer player, int oldLife, int newLife)
    {
        WriteColoredLine("TODO", ConsoleColor.White);
    }

    public void OnCardMovedZone(ICard card, PlayZone fromZone, PlayZone toZone)
    {
        WriteColoredLine("TODO", ConsoleColor.White);
    }

    public void InitializeVisuals(GameContext context)
    {
        WriteColoredLine("TODO", ConsoleColor.White);
    }
}
