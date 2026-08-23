using Godot;
using MTG.Core.Enums;
using MTG.Engine.Enums;
using MTG.Engine.Gameplay;
using System;
using System.Threading.Tasks;

namespace MTG.Frontend;

public class GodotInputProvider : IPlayerInputProvider
{
    private readonly RichTextLabel _gameLog;
    private readonly LineEdit _playerInput;
    private TaskCompletionSource<string> _inputTcs;

    public GodotInputProvider(RichTextLabel gameLog, LineEdit playerInput)
    {
        _gameLog = gameLog ?? throw new ArgumentNullException(nameof(gameLog));
        _playerInput = playerInput ?? throw new ArgumentNullException(nameof(playerInput));

        Callable.From(() =>
        {
            _playerInput.TextSubmitted += OnTextSubmitted;
            _playerInput.FocusExited += OnFocusExited;
        }).CallDeferred();
    }

    private void OnFocusExited()
    {
        Callable.From(() => _playerInput?.GrabFocus()).CallDeferred();
    }

    public async Task<PlayerAction> GetNextAction(GameContext context, CommanderPlayer player)
    {
        if (context.TurnStep == TurnStep.Untap)
        {
            return new PlayerAction(player, ActionType.PassPriority);
        }

        bool holdsStackPriority = context.StackCount > 0;

        if (holdsStackPriority)
        {
            return await GetCastSpellReactionAsync(context, player);
        }

        if (player == context.ActivePlayer && (context.TurnStep == TurnStep.Main1 || context.TurnStep == TurnStep.Main2))
        {
            return await GetMainStepActionAsync(context, player);
        }

        return await GetPriorityActionAsync(context, player);
    }

    private async Task<PlayerAction> GetMainStepActionAsync(GameContext context, CommanderPlayer player)
    {
        while (true)
        {
            LogUi($"\n[color=green]► [{context.TurnStep}] {player.Name}, it's your main phase.[/color]\n" +
                  $"[color=gray]1: Play a Card from Hand | 2: Show Board | 3: Pass Priority (End Phase)[/color]\n");

            string input = await WaitForUserInputAsync();

            switch (input.Trim())
            {
                case "1":
                    var selectedCard = await ChooseHandCardAsync(context, player);
                    if (selectedCard != null)
                    {
                        return new PlayerAction(player, ActionType.PlayCard, selectedCard);
                    }
                    continue;

                case "2":
                    LogUi($"[color=light_blue]{context.ToConsoleBattlefield()}[/color]\n");
                    continue;

                case "3":
                    return new PlayerAction(player, ActionType.PassPriority);

                default:
                    LogUi("[color=orange]Invalid selection. Type 1, 2, or 3.[/color]\n");
                    continue;
            }
        }
    }

    private async Task<PlayerAction> GetCastSpellReactionAsync(GameContext context, CommanderPlayer player)
    {
        while (true)
        {
            var topStackCard = context.PeekStack();
            string casterName = topStackCard.Owner.Name;

            LogUi($"\n[color=magenta]⚡ [{casterName}] casted {topStackCard.CardData.FullName}[/color]\n" +
                  $"[color=yellow][{player.Name}] How do you react?[/color]\n" +
                  $"[color=gray]1: Play a Card from Hand | 2: Show Stack | 3: Pass Priority[/color]\n");

            string input = await WaitForUserInputAsync();

            switch (input.Trim())
            {
                case "1":
                    var selectedCard = await ChooseHandCardAsync(context, player);
                    if (selectedCard != null)
                    {
                        return new PlayerAction(player, ActionType.PlayCard, selectedCard);
                    }
                    continue;

                case "2":
                    LogUi($"[color=light_blue]{context.ToConsoleStack()}[/color]\n");
                    continue;

                case "3":
                    return new PlayerAction(player, ActionType.PassPriority);

                default:
                    LogUi("[color=orange]Invalid selection. Type 1, 2, or 3.[/color]\n");
                    continue;
            }
        }
    }

    private async Task<PlayerAction> GetPriorityActionAsync(GameContext context, CommanderPlayer player)
    {
        while (true)
        {
            LogUi($"\n[color=yellow]► [{context.TurnStep}] Priority: {player.Name}[/color]\n" +
                  $"[color=gray]1: Activate Instant / Ability | 2: Show Board | 3: Pass Priority[/color]\n");

            string input = await WaitForUserInputAsync();

            switch (input.Trim())
            {
                case "1":
                    var selectedCard = await ChooseHandCardAsync(context, player);
                    if (selectedCard != null)
                    {
                        return new PlayerAction(player, ActionType.PlayCard, selectedCard);
                    }
                    continue;

                case "2":
                    LogUi($"[color=light_blue]{context.ToConsoleBattlefield()}[/color]\n");
                    continue;

                case "3":
                    return new PlayerAction(player, ActionType.PassPriority);

                default:
                    LogUi("[color=orange]Invalid selection. Type 1, 2, or 3.[/color]\n");
                    continue;
            }
        }
    }

    private async Task<CardInstance?> ChooseHandCardAsync(GameContext context, CommanderPlayer player)
    {
        if (player.Hand.Count == 0)
        {
            LogUi("[color=orange]Your hand is empty![/color]\n");
            return null;
        }

        while (true)
        {
            LogUi($"\n[color=white]{player.Name}, select a card to play:[/color]\n");
            for (int i = 0; i < player.Hand.Count; i++)
            {
                var card = player.Hand[i];
                LogUi($"[color=green]{i + 1}:[/color] {card.CardData.FullName} | ");
            }
            LogUi($"[color=red]{player.Hand.Count + 1}: Return[/color]\n");

            string input = await WaitForUserInputAsync();

            if (int.TryParse(input.Trim(), out int choice) && choice >= 1 && choice <= player.Hand.Count + 1)
            {
                if (choice == player.Hand.Count + 1)
                {
                    return null;
                }
                return player.Hand[choice - 1];
            }

            LogUi("[color=orange]Invalid card index. Try again.[/color]\n");
        }
    }

    private Task<string> WaitForUserInputAsync()
    {
        _inputTcs = new TaskCompletionSource<string>();

        Callable.From(() =>
        {
            _playerInput.Editable = true;
            _playerInput.GrabFocus();
        }).CallDeferred();

        return _inputTcs.Task;
    }

    private void OnTextSubmitted(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return;

        LogUi($"[color=green]> {text}[/color]\n");

        Callable.From(() =>
        {
            _playerInput.Clear();
            _playerInput.GrabFocus();
        }).CallDeferred();

        _inputTcs?.TrySetResult(text);
    }

    private void LogUi(string text)
    {
        Callable.From(() =>
        {
            _gameLog?.AppendText(text);
            _gameLog?.ScrollToLine(_gameLog.GetLineCount());
        }).CallDeferred();
    }
}
