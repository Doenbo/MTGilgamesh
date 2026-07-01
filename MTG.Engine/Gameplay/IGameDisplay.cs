using MTG.Core.Cards;
using MTG.Engine.Enums;
using MTG.Core.Enums;
using System.Security.Policy;

namespace MTG.Engine.Gameplay;

public interface IGameDisplay
{
    void LogMessage(string message);
    void LogStepTransition(TurnStep name, string playerName);
    void LogElimination(string playerName);

    void OnCardPlayed(CommanderPlayer player, ICard card);
    void OnCardTapped(ICard card, bool isTapped);
    void OnLifeTotalChanged(CommanderPlayer player, int oldLife, int newLife);
    void OnCardMovedZone(ICard card, PlayZone fromZone, PlayZone toZone);

    void InitializeVisuals(GameContext context);
}
