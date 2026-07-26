using Godot;
using MTG.Engine.Enums;
using MTG.Engine.Gameplay;
using System;
using System.Threading.Tasks;

public class GodotInputProvider : IPlayerInputProvider
{
    private readonly RichTextLabel _gameLog;
    private readonly LineEdit _playerInput;
    private TaskCompletionSource<string> _inputTcs;

    public GodotInputProvider(RichTextLabel gameLog, LineEdit playerInput)
    {
        _gameLog = gameLog;
        _playerInput = playerInput;

        _playerInput.TextSubmitted += OnTextSubmitted;
    }

    public async Task<PlayerAction> GetNextAction(GameContext context, CommanderPlayer player)
    {
        bool holdsStackPriority = context.StackCount > 0;

        if (context.TurnStep == TurnStep.Untap)
        {
            return new PlayerAction(player, ActionType.PassPriority);
        }

        if (holdsStackPriority) return GetCastSpellReaction(context, player);
        if (player == context.ActivePlayer && (context.TurnStep == TurnStep.Main1 || context.TurnStep == TurnStep.Main2))
        {
            return await GetMainStepAction(context, player);
        }

        return GetPriorityAction(context, player);
    }

    private async Task<PlayerAction> GetMainStepAction(GameContext context, CommanderPlayer player)
    {
        while (true)
        {
            _gameLog.AppendText($"\n[{context.TurnStep}] {context.PriorityPlayer.Name}, it's your main phase. What do you do?\n");
            _gameLog.AppendText("1: Play a Card from your Hand | 2: Show Board | 3: Pass Priority (End Phase)\n");

            string input = await WaitForUserInputAsync();

            switch (input)
            {
                case "1":
                    var input2 = ChooseHandCard(context, player);
                    if (!int.TryParse(input2, out int j) || j < 1 || j > player.Hand.Count + 1)
                    {
                        _gameLog.AppendText("Could not process input. Try again!\n");
                        continue;
                    }

                    if (j == player.Hand.Count + 1)
                        continue;

                    return new PlayerAction(player, ActionType.PlayCard, player.Hand[j - 1]);

                case "2":
                    _gameLog.AppendText($"{context.ToConsoleBattlefield()}\n");
                    continue;
                case "3":
                    return new PlayerAction(player, ActionType.PassPriority);
                default:
                    _gameLog.AppendText("Could not process input. Try again!\n");
                    continue;
            }
        }
    }

    private PlayerAction GetCastSpellReaction(GameContext context, CommanderPlayer player)
    {
        while (true)
        {
            var topStackCard = context.PeekStack();
            string casterName = topStackCard.Owner.Name;

            _gameLog.AppendText($"\n[{casterName}] has casted {topStackCard.CardData.FullName}\n");
            _gameLog.AppendText($"[{player.Name}] How do you react?\n");
            _gameLog.AppendText("1: Play a Card from your Hand | 2: Show Stack | 3: Pass Priority\n");

            var input = _playerInput.Text;

            if (input == "1")
            {
                var input2 = ChooseHandCard(context, player);
                if (!int.TryParse(input2, out int j) || j < 1 || j > player.Hand.Count + 1)
                {
                    _gameLog.AppendText("Could not process input. Try again!\n");
                    continue;
                }

                if (j == player.Hand.Count + 1)
                    continue;

                return new PlayerAction(player, ActionType.PlayCard, player.Hand[j - 1]);
            }
            if (input == "2")
            {
                _gameLog.AppendText($"{context.ToConsoleStack()}\n");
                continue;
            }
            if (input == "3")
            {
                return new PlayerAction(player, ActionType.PassPriority);
            }

            _gameLog.AppendText("Could not process input. Try again!\n");
        }
    }

    private PlayerAction GetPriorityAction(GameContext context, CommanderPlayer player)
    {
        while (true)
        {
            _gameLog.AppendText($"\n[{context.TurnStep}] Priority: {player.Name}. What do you do?\n");
            _gameLog.AppendText("1: Activate Instant / Ability | 2: Pass Priority\n");

            var input = _playerInput.Text;

            if (input == "1")
            {
                var input2 = ChooseHandCard(context, player);
                if (!int.TryParse(input2, out int j) || j < 1 || j > player.Hand.Count + 1)
                {
                    _gameLog.AppendText("Could not process input. Try again!\n");
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

            _gameLog.AppendText("Could not process input. Try again!\n");
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

        return _playerInput.Text;
    }

    private Task<string> WaitForUserInputAsync()
    {
        _inputTcs = new TaskCompletionSource<string>();

        _playerInput.Editable = true;
        _playerInput.GrabFocus();

        return _inputTcs.Task;
    }

    private void OnTextSubmitted(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return;

        _gameLog.AppendText($"[color=cyan]> {text}[/color]\n");
        _playerInput.Clear();

        _inputTcs?.TrySetResult(text);
    }
}
