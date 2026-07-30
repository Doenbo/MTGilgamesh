using Godot;
using Microsoft.Extensions.Logging;
using MTG.Core.Helper;
using MTG.Engine.Gameplay;
using MTG.Opponent;
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
		GD.Print(">>> Starting MTGilgamesh Godot Frontend <<<");

		CreateUi();
		_ = StartGameAsync();
	}

	private void CreateUi()
	{
		var canvasLayer = new CanvasLayer();
		AddChild(canvasLayer);

		// Main vertical layout for top logs section vs bottom input section
		var mainVBox = new VBoxContainer();
		mainVBox.SetAnchorsPreset(Control.LayoutPreset.FullRect);
		mainVBox.AnchorRight = 1.0f;
		mainVBox.AnchorBottom = 1.0f;
		mainVBox.FocusMode = Control.FocusModeEnum.None;
		mainVBox.AddThemeConstantOverride("separation", 10);
		canvasLayer.AddChild(mainVBox);

		// Top row: Game Log (Left) & Dev Log (Right) taking ~85-90% vertical space
		var topHBox = new HBoxContainer();
		topHBox.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
		topHBox.SizeFlagsVertical = Control.SizeFlags.ExpandFill;
		topHBox.SizeFlagsStretchRatio = 8.5f;
		topHBox.AddThemeConstantOverride("separation", 10);
		mainVBox.AddChild(topHBox);

		// Bottom row: Input Command (Left) & LineEdit (Right) taking sleek compact height
		var bottomHBox = new HBoxContainer();
		bottomHBox.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
		bottomHBox.SizeFlagsVertical = Control.SizeFlags.ShrinkEnd;
		bottomHBox.SizeFlagsStretchRatio = 1.0f;
		bottomHBox.CustomMinimumSize = new Vector2(0, 45);
		bottomHBox.AddThemeConstantOverride("separation", 10);
		mainVBox.AddChild(bottomHBox);

		GameLog ??= new RichTextLabel();
		DevLog ??= new RichTextLabel();
		PlayerInputCommand ??= new Label();
		PlayerInput ??= new LineEdit();

		// Enable BBCode & Auto-Scrolling to bottom when text is appended
		GameLog.BbcodeEnabled = true;
		GameLog.ScrollFollowing = true;

		DevLog.BbcodeEnabled = true;
		DevLog.ScrollFollowing = true;

		GameLog.FocusMode = Control.FocusModeEnum.None;
		GameLog.GetVScrollBar().FocusMode = Control.FocusModeEnum.None;

		DevLog.FocusMode = Control.FocusModeEnum.None;
		DevLog.GetVScrollBar().FocusMode = Control.FocusModeEnum.None;

		PlayerInputCommand.FocusMode = Control.FocusModeEnum.None;

		GameLog.MouseFilter = Control.MouseFilterEnum.Ignore;
		DevLog.MouseFilter = Control.MouseFilterEnum.Ignore;
		PlayerInputCommand.MouseFilter = Control.MouseFilterEnum.Ignore;

		PlayerInputCommand.Text = " Command Input:";
		PlayerInputCommand.VerticalAlignment = VerticalAlignment.Center;
		PlayerInput.PlaceholderText = "Type choice (e.g., 1, 2, 3) and press Enter...";

		PlayerInput.Editable = true;
		PlayerInput.FocusMode = Control.FocusModeEnum.All;
		PlayerInput.MouseFilter = Control.MouseFilterEnum.Stop;

		// Top logs sizing inside topHBox
		GameLog.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
		GameLog.SizeFlagsVertical = Control.SizeFlags.ExpandFill;
		GameLog.SizeFlagsStretchRatio = 1.0f;

		DevLog.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
		DevLog.SizeFlagsVertical = Control.SizeFlags.ExpandFill;
		DevLog.SizeFlagsStretchRatio = 1.0f;

		topHBox.AddChild(GameLog);
		topHBox.AddChild(DevLog);

		// Bottom controls sizing inside bottomHBox
		PlayerInputCommand.SizeFlagsHorizontal = Control.SizeFlags.ShrinkBegin;
		PlayerInputCommand.SizeFlagsVertical = Control.SizeFlags.ShrinkCenter;
		PlayerInputCommand.CustomMinimumSize = new Vector2(140, 40);

		PlayerInput.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
		PlayerInput.SizeFlagsVertical = Control.SizeFlags.ShrinkCenter;
		PlayerInput.CustomMinimumSize = new Vector2(300, 40);

		bottomHBox.AddChild(PlayerInputCommand);
		bottomHBox.AddChild(PlayerInput);

		Callable.From(() => PlayerInput.GrabFocus()).CallDeferred();
	}

	private async Task StartGameAsync()
	{
		GameLog.AppendText("[color=yellow]=== MTGilgamesh Engine Loading... ===[/color]\n");

		_loggerFactory = LoggerFactory.Create(builder =>
		{
			builder.AddProvider(new GodotUiLoggerProvider(DevLog));
		});

		LogManager.Factory = _loggerFactory;

		IPlayerInputProvider pip = new GodotInputProvider(GameLog, PlayerInput);
		IPlayerInputProvider pop = new OpponentInputProvider();

		var contextResult = await GameContext.Create(pip, pop);

		if (contextResult.IsFailure)
		{
			GameLog.AppendText($"[color=red]Failed to initialize game context: {contextResult.Error}[/color]\n");
			return;
		}

		var context = contextResult.Value;

		IGameDisplay display = new GodotGameDisplay(GameLog);

		var engine = new GameEngine(context, display);

		GameLog.AppendText("[color=green]=== Game Loop Started! ===[/color]\n");

		// Run engine game loop on background task to keep Godot UI thread responsive
		await Task.Run(() => engine.StartGameLoop());
	}

	public override void _Process(double delta)
	{
		if (PlayerInput != null && PlayerInput.Editable && !PlayerInput.HasFocus())
		{
			PlayerInput.GrabFocus();
		}
	}
}
