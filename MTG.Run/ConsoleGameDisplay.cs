using MTG.Core.Cards;
using MTG.Engine.Enums;
using MTG.Engine.Events;
using MTG.Engine.Gameplay;

namespace MTG.Run;

public class ConsoleGameDisplay : IGameDisplay
{
    public bool IsLoggingErrors { get; set; } = true;

    public void LogInfo(string message) => Console.WriteLine(message);

    public void LogError(string message) { if (IsLoggingErrors) Console.WriteLine(message); }

    public void LogGameEvent(IGameEvent gameEvent)
    {
        switch (gameEvent)
        {
            case StepTransitionEvent step:
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"\n=== {step.Description} ===");
                break;

            case SpellCastEvent spell:
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"[CAST] {spell.PlayerName} played {spell.SpellName}");
                break;

            case DamageDealtEvent damage:
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[DAMAGE] {damage.Source} -> {damage.Target}: {damage.Amount}");
                break;

            default:
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine(gameEvent.Description);
                break;
        }

        Console.ResetColor();
    }

    public void RenderBattlefield(GameContext context)
    {
        Console.WriteLine($"{context.ToConsoleBattlefield()}");
    }

    public void RenderManaPool(GameContext context)
    {
        Console.WriteLine($"{context.ToConsoleManaPool()}");
    }

    public void RenderStack(GameContext context)
    {
        Console.WriteLine($"{context.ToConsoleStack()}");
    }

    public void OnCardPlayed(CommanderPlayer player, ICard card)
    {
        throw new NotImplementedException();
    }

    public void OnCardTapped(ICard card, bool isTapped)
    {
        throw new NotImplementedException();
    }

    public void OnLifeTotalChanged(CommanderPlayer player, int oldLife, int newLife)
    {
        throw new NotImplementedException();
    }

    public void OnCardMovedZone(ICard card, PlayZone fromZone, PlayZone toZone)
    {
        throw new NotImplementedException();
    }

    public void InitializeVisuals(GameContext context)
    {
        throw new NotImplementedException();
    }
}
