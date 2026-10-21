using MTG.Core.Cards;
using MTG.Core.Helper;
using MTG.Engine.Gameplay;
using MTG.Core.Tests.Dummy;
using MTG.Engine.Enums;
using MTG.Engine.Cheating;

namespace MTG.Engine.Tests.Dummy;

public class EngineDummyFactory
{
    public static Result<CommanderPlayer> CreateCommanderPlayer(List<ICard> comms, string name = "TestPlayer", int life = 40)
    {
        var d1 = CoreDummyFactory.CreateCommanderDeck(comms);
        if (d1.IsFailure) return d1.ToFailure<CommanderPlayer>();

        var d2 = CreatePlayerInputProvider();
        if (d2.IsFailure) return d2.ToFailure<CommanderPlayer>();

        return CommanderPlayer.Create(name, life, d1.Value, d2.Value, Cheats.None);
    }

    public static Result<IPlayerInputProvider> CreatePlayerInputProvider()
    {
        return Result<IPlayerInputProvider>.Success(new DummyInputProvider());
    }

    private class DummyInputProvider : IPlayerInputProvider
    {
        public async Task<PlayerAction> GetNextAction(GameContext context, CommanderPlayer player)
        {
            return new PlayerAction(player, ActionType.PassPriority);
        }
    }
}
