using MTG.Core.Cards;
using MTG.Engine.Enums;
using MTG.Engine.Events;

namespace MTG.Engine.Gameplay;

public interface IGameDisplay
{
    //Logging
    bool IsLoggingErrors { get; set; }

    void LogInfo(string message);
    void LogError(string message);
    void LogGameEvent(IGameEvent gameEvent);

    void RenderBattlefield(GameContext context);
    void RenderManaPool(GameContext context);
    void RenderStack(GameContext context);

    // Optional visual event hooks with default empty implementations
    void OnCardPlayed(CommanderPlayer player, ICard card) { }
    void OnCardTapped(ICard card, bool isTapped) { }
    void OnLifeTotalChanged(CommanderPlayer player, int oldLife, int newLife) { }
    void OnCardMovedZone(ICard card, PlayZone fromZone, PlayZone toZone) { }
    void InitializeVisuals(GameContext context) { }
}
