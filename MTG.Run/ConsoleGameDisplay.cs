using MTG.Core.Cards;
using MTG.Engine.Enums;
using MTG.Engine.Gameplay;

namespace MTG.Run;

public class ConsoleGameDisplay : IGameDisplay
{
    public void LogMessage(string message) => Console.WriteLine(message);

    public void LogStepTransition(TurnStep name, string playerName)
    {
        Console.WriteLine($"\n--- [{name}] for {playerName} ---");
    }

    public void LogElimination(string playerName)
    {
        Console.WriteLine($"\n [ELIMINATION] {playerName} has 0 Life and is eliminated!");
    }

    public void RenderBattlefield(GameContext context)
    {
        // Loop to render Board to Console
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
