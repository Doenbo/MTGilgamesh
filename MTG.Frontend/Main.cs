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
	[Export] public Label PlayerInputCommand { get; set; }
	[Export] public LineEdit PlayerInput { get; set; }

	private ILoggerFactory _loggerFactory;

	public override void _Ready()
	{
		GD.Print(">>> START SKRIPT! <<<");

		CreateUi();
		_ = StartGame();
	}

	private void CreateUi()
	{
		// 1. CanvasLayer for a fixed UI overlay
		var canvasLayer = new CanvasLayer();
		AddChild(canvasLayer);

		// 2. Main Grid Container over the full screen
		var grid = new GridContainer();
		grid.Columns = 2;
		grid.SetAnchorsPreset(Control.LayoutPreset.FullRect);
		grid.AnchorRight = 1.0f;
		grid.AnchorBottom = 1.0f;
		grid.FocusMode = Control.FocusModeEnum.None;
		canvasLayer.AddChild(grid);

		// 3. Fallback instantiation
		GameLog ??= new RichTextLabel();
		DevLog ??= new RichTextLabel();
		PlayerInputCommand ??= new Label();
		PlayerInput ??= new LineEdit();

		// 4. Formats & Focus Policy
		GameLog.BbcodeEnabled = true;
		DevLog.BbcodeEnabled = true;

		// Prevent passive controls from stealing focus
		GameLog.FocusMode = Control.FocusModeEnum.None;
		DevLog.FocusMode = Control.FocusModeEnum.None;
		PlayerInputCommand.FocusMode = Control.FocusModeEnum.None;

		GameLog.MouseFilter = Control.MouseFilterEnum.Ignore;
		DevLog.MouseFilter = Control.MouseFilterEnum.Ignore;
		PlayerInputCommand.MouseFilter = Control.MouseFilterEnum.Ignore;

		PlayerInputCommand.Text = "Input Command:";
		PlayerInput.PlaceholderText = "Enter command...";

		// 5. LineEdit Input Setup
		PlayerInput.Editable = true;
		PlayerInput.FocusMode = Control.FocusModeEnum.All;
		PlayerInput.MouseFilter = Control.MouseFilterEnum.Stop;

		// Minimum sizes for GridContainer calculations
		GameLog.CustomMinimumSize = new Vector2(200, 100);
		DevLog.CustomMinimumSize = new Vector2(200, 100);
		PlayerInputCommand.CustomMinimumSize = new Vector2(150, 40);
		PlayerInput.CustomMinimumSize = new Vector2(200, 40);

		// 6. Assignment to the 4 quadrants
		ConfigureAndAdd(grid, GameLog);          // Quadrant 1: Top-Left
		ConfigureAndAdd(grid, DevLog);           // Quadrant 2: Top-Right
		ConfigureAndAdd(grid, PlayerInputCommand);  // Quadrant 3: Bottom-Left
		ConfigureAndAdd(grid, PlayerInput);      // Quadrant 4: Bottom-Right

		// Grab focus for input field on startup
		Callable.From(() => PlayerInput.GrabFocus()).CallDeferred();
	}

	private async Task StartGame()
	{
		GameLog.AppendText("[color=white]Start Main.cs StartGame[/color]\n");

		_loggerFactory = LoggerFactory.Create(builder =>
		{
			builder.AddProvider(new GodotUiLoggerProvider(DevLog));
		});

		LogManager.Factory = _loggerFactory;

		var context = await GameContext.Create();

		IGameDisplay consoleDisplay = new GodotGameDisplay();
		IPlayerInputProvider consoleInput = new GodotInputProvider(GameLog, PlayerInput);

		var engine = new GameEngine(context.Value, consoleDisplay, consoleInput);

		engine.StartGameLoop();
	}

	/// <summary>
	/// Configures size flags so the control expands fully within its grid cell.
	/// </summary>
	private void ConfigureAndAdd(GridContainer grid, Control control)
	{
		control.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
		control.SizeFlagsVertical = Control.SizeFlags.ExpandFill;
		grid.AddChild(control);
	}

	public override void _Process(double delta)
	{

	}
}
