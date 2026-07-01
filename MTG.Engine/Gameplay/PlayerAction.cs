using MTG.Engine.Enums;

namespace MTG.Engine.Gameplay;

public class PlayerAction
{
    public CommanderPlayer Player { get; }
    public ActionType Type { get; }
    public CardInstance? TargetCardInstance { get; }

    public PlayerAction(CommanderPlayer player, ActionType type)
    {
        Player = player;
        Type = type;
        TargetCardInstance = null;
    }

    public PlayerAction(CommanderPlayer player, ActionType type, CardInstance targetCardInstance)
    {
        Player = player;
        Type = type;
        TargetCardInstance = targetCardInstance;
    }
}
