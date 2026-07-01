using MTG.Core.Cards;
using MTG.Core.Enums;
using MTG.Engine.Enums;
using MTG.Engine.Gameplay;

internal class GodotGameDisplay : IGameDisplay
{
	public void InitializeVisuals(GameContext context)
	{
		throw new System.NotImplementedException();
	}

	public void LogElimination(string playerName)
	{
		throw new System.NotImplementedException();
	}

	public void LogMessage(string message)
	{
		throw new System.NotImplementedException();
	}

	public void LogStepTransition(TurnStep name, string playerName)
	{
		throw new System.NotImplementedException();
	}

	public void OnCardMovedZone(ICard card, PlayZone fromZone, PlayZone toZone)
	{
		throw new System.NotImplementedException();
	}

	public void OnCardPlayed(CommanderPlayer player, ICard card)
	{
		throw new System.NotImplementedException();
	}

	public void OnCardTapped(ICard card, bool isTapped)
	{
		throw new System.NotImplementedException();
	}

	public void OnLifeTotalChanged(CommanderPlayer player, int oldLife, int newLife)
	{
		throw new System.NotImplementedException();
	}
}
