using Godot;
using Microsoft.Extensions.Logging;
using MTG.Core.Helper;
using MTG.Engine.Gameplay;
using MTG.Resources.Enums;
using System;
using System.Threading.Tasks;

namespace MTG.Frontend;

public partial class Main : Node2D
{
	[Export] public RichTextLabel GameLog { get; set; }
	[Export] public RichTextLabel DevLog { get; set; }
	[Export] public Label PlayInputCommand { get; set; }
	[Export] public LineEdit PlayerInput { get; set; }
	
	public override void _Ready()
	{
		_ = StartGame();
	}

	private async Task StartGame()
	{
		GameLog.AppendText("[color=white]Start Main.cs StartGame[/color]\n");

        using var uiLoggerFactory = LoggerFactory.Create(builder =>
		{
			builder.AddProvider(new GodotUiLoggerProvider(DevLog));
		});

		LogManager.Factory = uiLoggerFactory;

		var context = await GameContext.Create();

		IGameDisplay consoleDisplay = new GodotGameDisplay();
		IPlayerInputProvider consoleInput = new GodotInputProvider(GameLog, PlayerInput);

		var engine = new GameEngine(context.Value, consoleDisplay, consoleInput);

		engine.StartGameLoop();
	}

	public override void _Process(double delta)
	{
		
	}
}
