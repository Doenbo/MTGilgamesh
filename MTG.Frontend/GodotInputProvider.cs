using Godot;
using MTG.Engine.Enums;
using MTG.Engine.Gameplay;
using System;
using System.Threading.Tasks;

public class GodotInputProvider : IPlayerInputProvider
{
	private readonly RichTextLabel _gameLog;
	private readonly LineEdit _playerInput;

	public GodotInputProvider(RichTextLabel gameLog, LineEdit playerInput)
	{
		_gameLog = gameLog;
		_playerInput = playerInput;
	}

	public async Task<PlayerAction> GetNextAction(GameContext context, CommanderPlayer player)
	{
		bool holdsStackPriority = context.StackCount > 0;
		bool isPhaseTransition = context.IsEndingTheStep;

		if (context.TurnStep == TurnStep.Untap ||
			context.TurnStep == TurnStep.Upkeep ||
			context.TurnStep == TurnStep.Draw ||
			context.TurnStep == TurnStep.EndStep ||
			context.TurnStep == TurnStep.CleanupStep)
		{
			return new PlayerAction(player, ActionType.GoToNextPhase);
		}

		if (context.TurnStep == TurnStep.CombatBegin ||
			context.TurnStep == TurnStep.DeclareAttackers ||
			context.TurnStep == TurnStep.DeclareBlockers ||
			context.TurnStep == TurnStep.CombatDamage ||
			context.TurnStep == TurnStep.EndOfCombat)
		{
			return new PlayerAction(player, ActionType.GoToNextPhase);
		}

		if (holdsStackPriority) return GetCastSpellReaction(context, player);
		if (isPhaseTransition) return GetPhaseTransitionReaction(context, player);
		if (player == context.ActivePlayer) return GetMainStepAction(context, player);

		throw new Exception("Unexpected game state.");
	}

	private PlayerAction GetMainStepAction(GameContext context, CommanderPlayer player)
	{
		while (true)
		{
			_gameLog.AppendText("$\"\\n[{context.TurnStep}] {context.PriorityPlayer.Name}, it's your turn. What do you do?\"");
			_gameLog.AppendText("1: Play a Card from your Hand | 2: Show Board | 3: Go to next Phase");

			//TODO
			var input = _playerInput.Text;

			switch (input)
			{
				case "1":
					var input2 = ChooseHandCard(context, player);
					if (!int.TryParse(input2, out int j) || j < 1 || j > player.Hand.Count + 1)
					{
						_gameLog.AppendText("Could not process input. Try again!");
						continue;
					}

					if (j == player.Hand.Count + 1)
						continue;

					return new PlayerAction(player, ActionType.PlayCard, player.Hand[j - 1]);

				case "2":
					_gameLog.AppendText($"{context.ToConsoleBattlefield()}");
					continue;
				case "3":
					return new PlayerAction(player, ActionType.GoToNextPhase);
				default:
					_gameLog.AppendText("Could not process input. Try again!");
					continue;
			}
		}
	}

	private PlayerAction GetCastSpellReaction(GameContext context, CommanderPlayer player)
	{
		while (true)
		{
			_gameLog.AppendText($"\n[{context.PriorityRoundInitiator.Name}] has casted {context.PeekStack().CardData.FullName}");
			_gameLog.AppendText($"[{player.Name}] How do you react?");
			_gameLog.AppendText("1: Play a Card from your Hand | 2: Show Stack | 3: Do not react");

			//TODO
			var input = _playerInput.Text;

			if (input == "1")
			{
				var input2 = ChooseHandCard(context, player);
				if (!int.TryParse(input2, out int j) || j < 1 || j > player.Hand.Count + 1)
				{
					_gameLog.AppendText("Could not process input. Try again!");
					continue;
				}

				if (j == player.Hand.Count + 1)
					continue;

				return new PlayerAction(player, ActionType.PlayCard, player.Hand[j - 1]);
			}
			if (input == "2")
			{
				_gameLog.AppendText($"{context.ToConsoleStack()}");
				continue;
			}
			if (input == "3")
			{
				return new PlayerAction(player, ActionType.PassPriority);
			}

			_gameLog.AppendText("Could not process input. Try again!");
		}
	}

	private PlayerAction GetPhaseTransitionReaction(GameContext context, CommanderPlayer player)
	{
		while (true)
		{
			_gameLog.AppendText($"\n[REACTION] {context.ActivePlayer.Name} wants to end '{context.TurnStep}'.");
			_gameLog.AppendText($"{player.Name}, how would you like to react?");
			_gameLog.AppendText("1: Activate Instant / Ability | 2: Pass");

			//TODO
			var input = _playerInput.Text;

			if (input == "1")
			{
				var input2 = ChooseHandCard(context, player);
				if (!int.TryParse(input2, out int j) || j < 1 || j > player.Hand.Count + 1)
				{
					_gameLog.AppendText("Could not process input. Try again!");
					continue;
				}

				if (j == player.Hand.Count + 1)
					continue;

				return new PlayerAction(player, ActionType.PlayCard, player.Hand[j - 1]);
			}
			if (input == "2")
			{
				return new PlayerAction(player, ActionType.PassPriority);
			}

			_gameLog.AppendText("Could not process input. Try again!");
		}
	}

	private string ChooseHandCard(GameContext context, CommanderPlayer player)
	{
		_gameLog.AppendText($"\n{context.PriorityPlayer.Name}, which card would you like to play from your hand?\n");
		for (int i = 0; i < player.Hand.Count; i++)
		{
			var c = player.Hand[i];
			_gameLog.AppendText($"{i + 1}: {c.CardData.FullName} | ");
		}
		_gameLog.AppendText($"{player.Hand.Count + 1}: Return\n");

		//TODO
		return _playerInput.Text;
	}
}
