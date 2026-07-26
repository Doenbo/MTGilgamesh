using MTG.Core.Cards;
using MTG.Core.Enums;
using MTG.Engine.Enums;

namespace MTG.Engine.Gameplay;

public interface IGameDisplay
{
    void LogMessage(string message);
    void LogStepTransition(TurnStep name, string playerName);
    void LogElimination(string playerName);

    // Optional visual event hooks with default empty implementations
    void OnCardPlayed(CommanderPlayer player, ICard card) { }
    void OnCardTapped(ICard card, bool isTapped) { }
    void OnLifeTotalChanged(CommanderPlayer player, int oldLife, int newLife) { }
    void OnCardMovedZone(ICard card, PlayZone fromZone, PlayZone toZone) { }
    void InitializeVisuals(GameContext context) { }
}
