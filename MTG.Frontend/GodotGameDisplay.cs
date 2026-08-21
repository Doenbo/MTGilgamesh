using Godot;
using MTG.Engine.Events;
using MTG.Engine.Gameplay;

namespace MTG.Frontend;

public class GodotGameDisplay : IGameDisplay
{
	private readonly RichTextLabel _gameLog;

	public bool IsLoggingErrors { get; set; } = true;

	public GodotGameDisplay(RichTextLabel gameLog)
	{
		_gameLog = gameLog ?? throw new System.ArgumentNullException(nameof(gameLog));
	}

	public void LogInfo(string message)
	{
		AppendBbcode($"[color=gainsboro]{message}[/color]\n");
	}

	private void AppendBbcode(string bbcode)
	{
		Callable.From(() =>
		{
			_gameLog?.AppendText(bbcode);
			_gameLog?.ScrollToLine(_gameLog.GetLineCount());
		}).CallDeferred();
	}

	public void LogError(string message)
	{
		
	}

	public void RenderBattlefield(GameContext context)
	{
		
	}

	public void LogGameEvent(IGameEvent gameEvent)
	{
		
	}

	public void RenderManaPool(GameContext context)
	{
		
	}

	public void RenderStack(GameContext context)
	{
		
	}
}
