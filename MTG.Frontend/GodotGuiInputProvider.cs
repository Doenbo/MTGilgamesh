using Godot;
using MTG.Engine.Enums;
using MTG.Engine.Gameplay;
using System;
using System.Threading.Tasks;

namespace MTG.Frontend;

public class GodotGuiInputProvider : IPlayerInputProvider
{
    private readonly Main _mainScene;
    private TaskCompletionSource<PlayerAction> _actionTcs;

    public GodotGuiInputProvider(Main mainScene)
    {
        _mainScene = mainScene ?? throw new ArgumentNullException(nameof(mainScene));
    }

    public async Task<PlayerAction> GetNextAction(GameContext context, CommanderPlayer player)
    {
        if (context.TurnStep == TurnStep.Untap)
        {
            return new PlayerAction(player, ActionType.PassPriority);
        }

        _actionTcs = new TaskCompletionSource<PlayerAction>();

        Callable.From(() =>
        {
            _mainScene.SetPriorityPrompt(player, context.TurnStep, context.StackCount > 0);
        }).CallDeferred();

        var action = await _actionTcs.Task;

        Callable.From(() =>
        {
            _mainScene.ClearPriorityPrompt();
        }).CallDeferred();

        return action;
    }

    public void OnPassPriorityPressed(CommanderPlayer player)
    {
        _actionTcs?.TrySetResult(new PlayerAction(player, ActionType.PassPriority));
    }

    public void OnCardClicked(CommanderPlayer player, CardInstance card)
    {
        _actionTcs?.TrySetResult(new PlayerAction(player, ActionType.PlayCard, card));
    }
}
