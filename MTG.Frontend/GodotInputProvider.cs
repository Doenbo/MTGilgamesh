using MTG.Engine.Gameplay;
using System.Threading.Tasks;

internal class GodotInputProvider : IPlayerInputProvider
{
	Task<PlayerAction> IPlayerInputProvider.GetNextAction(GameContext context, CommanderPlayer player)
	{
		throw new System.NotImplementedException();
	}
}
