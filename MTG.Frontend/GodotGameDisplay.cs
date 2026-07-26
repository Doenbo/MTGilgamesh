using Godot;
using MTG.Engine.Enums;
using MTG.Engine.Gameplay;

namespace MTG.Frontend;

public class GodotGameDisplay : IGameDisplay
{
    private readonly RichTextLabel _gameLog;

    public GodotGameDisplay(RichTextLabel gameLog)
    {
        _gameLog = gameLog ?? throw new System.ArgumentNullException(nameof(gameLog));
    }

    public void LogMessage(string message)
    {
        AppendBbcode($"[color=gainsboro]{message}[/color]\n");
    }

    public void LogStepTransition(TurnStep name, string playerName)
    {
        AppendBbcode($"\n[color=cyan]━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━[/color]\n" +
                     $"[color=yellow]► [{name}] — {playerName}'s Turn[/color]\n" +
                     $"[color=cyan]━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━[/color]\n");
    }

    public void LogElimination(string playerName)
    {
        AppendBbcode($"\n[color=red][bold]☠ {playerName} has been ELIMINATED! ☠[/bold][/color]\n");
    }

    private void AppendBbcode(string bbcode)
    {
        Callable.From(() =>
        {
            _gameLog?.AppendText(bbcode);
            _gameLog?.ScrollToLine(_gameLog.GetLineCount());
        }).CallDeferred();
    }
}
