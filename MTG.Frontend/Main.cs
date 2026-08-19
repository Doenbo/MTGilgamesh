using Godot;
using Microsoft.Extensions.Logging;
using MTG.Core.Helper;
using MTG.Engine.Enums;
using MTG.Engine.Gameplay;
using MTG.Opponent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MTG.Frontend;

public partial class Main : Node2D
{
	[Export] public RichTextLabel GameLog { get; set; }
	[Export] public RichTextLabel DevLog { get; set; }

	private GameContext _context;
	private GodotGuiInputProvider _guiInput;

	// Board Navigation State
	private int _viewingPlayerIndex = 0; // 0 = Human (Dön), 1 = Bot 1, 2 = Bot 2, 3 = Bot 3
	private Label _boardTitleLabel;
	private Button _prevBoardBtn;
	private Button _nextBoardBtn;

	// Fullscreen Board UI
	private VBoxContainer _battlefieldContainer;
	private Label _battlefieldEmptyLabel;

	// Bottom Human HUD
	private Label _humanLifeLabel;
	private Label _humanManaLabel;
	private HBoxContainer _handContainer;
	private Button _passPriorityBtn;

	// Turn & Priority Status Bar
	private Label _stepBannerLabel;

	private ILoggerFactory _loggerFactory;

	public override void _Ready()
	{
		GD.Print(">>> Starting MTGilgamesh Fullscreen Godot GUI <<<");

		CreateUi();
		_ = StartGameAsync();
	}

	private void CreateUi()
	{
		var canvasLayer = new CanvasLayer();
		AddChild(canvasLayer);

		var mainVBox = new VBoxContainer();
		mainVBox.SetAnchorsPreset(Control.LayoutPreset.FullRect);
		mainVBox.AnchorRight = 1.0f;
		mainVBox.AnchorBottom = 1.0f;
		mainVBox.AddThemeConstantOverride("separation", 6);
		canvasLayer.AddChild(mainVBox);

		// ==========================================
		// 1. TOP BAR: Title & Board Switcher (Top-Right Arrows)
		// ==========================================
		var topBarHBox = new HBoxContainer();
		topBarHBox.CustomMinimumSize = new Vector2(0, 45);
		topBarHBox.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
		mainVBox.AddChild(topBarHBox);

		_stepBannerLabel = new Label();
		_stepBannerLabel.Text = " [Untap Step] ";
		_stepBannerLabel.AddThemeFontSizeOverride("font_size", 16);
		_stepBannerLabel.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
		_stepBannerLabel.VerticalAlignment = VerticalAlignment.Center;
		topBarHBox.AddChild(_stepBannerLabel);

		// Top-Right Board Switcher Controls
		var boardNavHBox = new HBoxContainer();
		boardNavHBox.AddThemeConstantOverride("separation", 6);
		topBarHBox.AddChild(boardNavHBox);

		_prevBoardBtn = new Button();
		_prevBoardBtn.Text = " ◄ ";
		_prevBoardBtn.CustomMinimumSize = new Vector2(40, 36);
		_prevBoardBtn.Pressed += OnPrevBoardPressed;
		boardNavHBox.AddChild(_prevBoardBtn);

		_boardTitleLabel = new Label();
		_boardTitleLabel.Text = " BOARD: Dön (You) ";
		_boardTitleLabel.AddThemeFontSizeOverride("font_size", 15);
		_boardTitleLabel.VerticalAlignment = VerticalAlignment.Center;
		boardNavHBox.AddChild(_boardTitleLabel);

		_nextBoardBtn = new Button();
		_nextBoardBtn.Text = " ► ";
		_nextBoardBtn.CustomMinimumSize = new Vector2(40, 36);
		_nextBoardBtn.Pressed += OnNextBoardPressed;
		boardNavHBox.AddChild(_nextBoardBtn);

		// ==========================================
		// 2. MIDDLE AREA: Fullscreen Active Board View + Logs Split
		// ==========================================
		var middleHSplit = new HBoxContainer();
		middleHSplit.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
		middleHSplit.SizeFlagsVertical = Control.SizeFlags.ExpandFill;
		middleHSplit.SizeFlagsStretchRatio = 7.0f;
		mainVBox.AddChild(middleHSplit);

		// Fullscreen Board Panel (Left 75% width)
		var boardPanel = new PanelContainer();
		boardPanel.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
		boardPanel.SizeFlagsVertical = Control.SizeFlags.ExpandFill;
		boardPanel.SizeFlagsStretchRatio = 3.0f;

		var boardStyle = new StyleBoxFlat();
		boardStyle.BgColor = new Color(0.08f, 0.1f, 0.13f, 0.95f);
		boardStyle.BorderWidthBottom = 2; boardStyle.BorderWidthLeft = 2;
		boardStyle.BorderWidthRight = 2; boardStyle.BorderWidthTop = 2;
		boardStyle.BorderColor = new Color(0.25f, 0.35f, 0.45f);
		boardPanel.AddThemeStyleboxOverride("panel", boardStyle);
		middleHSplit.AddChild(boardPanel);

		var boardVBox = new VBoxContainer();
		boardPanel.AddChild(boardVBox);

		_battlefieldEmptyLabel = new Label();
		_battlefieldEmptyLabel.Text = "\n\n   (Battlefield is currently empty)";
		_battlefieldEmptyLabel.HorizontalAlignment = HorizontalAlignment.Center;
		_battlefieldEmptyLabel.AddThemeFontSizeOverride("font_size", 14);
		_battlefieldEmptyLabel.Modulate = new Color(0.6f, 0.6f, 0.6f);
		boardVBox.AddChild(_battlefieldEmptyLabel);

		_battlefieldContainer = new VBoxContainer();
		_battlefieldContainer.SizeFlagsVertical = Control.SizeFlags.ExpandFill;
		boardVBox.AddChild(_battlefieldContainer);

		// Right Log Panel (Right 25% width)
		var logVBox = new VBoxContainer();
		logVBox.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
		logVBox.SizeFlagsVertical = Control.SizeFlags.ExpandFill;
		logVBox.SizeFlagsStretchRatio = 1.0f;
		middleHSplit.AddChild(logVBox);

		GameLog ??= new RichTextLabel();
		GameLog.BbcodeEnabled = true;
		GameLog.ScrollFollowing = true;
		GameLog.SizeFlagsVertical = Control.SizeFlags.ExpandFill;
		GameLog.FocusMode = Control.FocusModeEnum.None;
		GameLog.GetVScrollBar().FocusMode = Control.FocusModeEnum.None;
		logVBox.AddChild(GameLog);

		DevLog ??= new RichTextLabel();
		DevLog.BbcodeEnabled = true;
		DevLog.ScrollFollowing = true;
		DevLog.SizeFlagsVertical = Control.SizeFlags.ExpandFill;
		DevLog.FocusMode = Control.FocusModeEnum.None;
		DevLog.GetVScrollBar().FocusMode = Control.FocusModeEnum.None;
		DevLog.Visible = false; // Collapsed by default
		logVBox.AddChild(DevLog);

		// ==========================================
		// 3. BOTTOM HUD: Human Player Hand & Priority Control Bar
		// ==========================================
		var hudPanel = new PanelContainer();
		hudPanel.CustomMinimumSize = new Vector2(0, 180);
		hudPanel.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
		hudPanel.SizeFlagsVertical = Control.SizeFlags.ShrinkEnd;

		var hudStyle = new StyleBoxFlat();
		hudStyle.BgColor = new Color(0.05f, 0.07f, 0.1f, 0.98f);
		hudStyle.BorderWidthTop = 3;
		hudStyle.BorderColor = new Color(0.8f, 0.65f, 0.2f); // Golden accent
		hudPanel.AddThemeStyleboxOverride("panel", hudStyle);
		mainVBox.AddChild(hudPanel);

		var hudHBox = new HBoxContainer();
		hudHBox.AddThemeConstantOverride("separation", 12);
		hudPanel.AddChild(hudHBox);

		// Player Stats Box
		var statsVBox = new VBoxContainer();
		statsVBox.CustomMinimumSize = new Vector2(160, 0);
		statsVBox.Alignment = BoxContainer.AlignmentMode.Center;
		hudHBox.AddChild(statsVBox);

		_humanLifeLabel = new Label();
		_humanLifeLabel.Text = "❤ Dön: 40 HP";
		_humanLifeLabel.AddThemeFontSizeOverride("font_size", 16);
		_humanLifeLabel.Modulate = new Color(0.3f, 1.0f, 0.4f);
		statsVBox.AddChild(_humanLifeLabel);

		_humanManaLabel = new Label();
		_humanManaLabel.Text = "Mana: {0}";
		_humanManaLabel.AddThemeFontSizeOverride("font_size", 13);
		statsVBox.AddChild(_humanManaLabel);

		// Scrollable Hand Container
		var handScroll = new ScrollContainer();
		handScroll.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
		handScroll.SizeFlagsVertical = Control.SizeFlags.ExpandFill;
		hudHBox.AddChild(handScroll);

		_handContainer = new HBoxContainer();
		_handContainer.AddThemeConstantOverride("separation", 8);
		handScroll.AddChild(_handContainer);

		// Action Bar
		var actionVBox = new VBoxContainer();
		actionVBox.CustomMinimumSize = new Vector2(160, 0);
		actionVBox.Alignment = BoxContainer.AlignmentMode.Center;
		hudHBox.AddChild(actionVBox);

		_passPriorityBtn = new Button();
		_passPriorityBtn.Text = "PASS PRIORITY";
		_passPriorityBtn.CustomMinimumSize = new Vector2(150, 50);
		_passPriorityBtn.AddThemeFontSizeOverride("font_size", 14);
		_passPriorityBtn.Pressed += OnPassPriorityPressed;
		actionVBox.AddChild(_passPriorityBtn);
	}

	private async Task StartGameAsync()
	{
		GameLog.AppendText("[color=yellow]=== Starting MTGilgamesh Engine ===[/color]\n");

		_loggerFactory = LoggerFactory.Create(builder =>
		{
			builder.AddProvider(new GodotUiLoggerProvider(DevLog));
		});

		LogManager.Factory = _loggerFactory;

		_guiInput = new GodotGuiInputProvider(this);
		IPlayerInputProvider botInput = new OpponentInputProvider();

		var contextResult = await GameContext.Create(_guiInput, botInput);

		if (contextResult.IsFailure)
		{
			GameLog.AppendText($"[color=red]Failed to initialize game context: {contextResult.Error}[/color]\n");
			return;
		}

		_context = contextResult.Value;

		IGameDisplay display = new GodotGameDisplay(GameLog);
		var engine = new GameEngine(_context, display);

		UpdateBoardView();
		UpdateHumanHand();

		GameLog.AppendText("[color=green]=== 4-Player Match Started! ===[/color]\n");

		await Task.Run(() => engine.StartGameLoop());
	}

	public void SetPriorityPrompt(CommanderPlayer priorityPlayer, TurnStep step, bool isStackActive)
	{
		_stepBannerLabel.Text = $" [{step}] — Priority: {priorityPlayer.Name} " + (isStackActive ? "(STACK ACTIVE)" : "");
		
		bool isHumanTurn = priorityPlayer == _context.Players[0];
		_passPriorityBtn.Disabled = !isHumanTurn;
		_passPriorityBtn.Modulate = isHumanTurn ? new Color(0.3f, 1.0f, 0.4f) : new Color(0.6f, 0.6f, 0.6f);

		UpdateHumanHand();
		UpdateBoardView();
	}

	public void ClearPriorityPrompt()
	{
		_passPriorityBtn.Disabled = true;
	}

	private void UpdateHumanHand()
	{
		if (_context == null || _handContainer == null) return;

		foreach (var child in _handContainer.GetChildren().ToList())
		{
			child.QueueFree();
		}

		var human = _context.Players[0];
		_humanLifeLabel.Text = $"❤ {human.Name}: {human.LifeTotal} HP";

		foreach (var cardInstance in human.Hand.ToList())
		{
			var cardNode = new CardNode();
			cardNode.Setup(cardInstance);
			cardNode.OnCardClicked = (node) =>
			{
				if (_context.PriorityPlayer == human)
				{
					_guiInput.OnCardClicked(human, node.CardInstance);
				}
			};
			_handContainer.AddChild(cardNode);
		}
	}

	private void UpdateBoardView()
	{
		if (_context == null || _battlefieldContainer == null) return;

		var targetPlayer = _context.Players[_viewingPlayerIndex];
		_boardTitleLabel.Text = $" BOARD: {targetPlayer.Name} {(_viewingPlayerIndex == 0 ? "(You)" : "[Bot]")} ";

		foreach (var child in _battlefieldContainer.GetChildren().ToList())
		{
			child.QueueFree();
		}

		var boardCards = _context.GetBoardOf(targetPlayer).ToList();
		_battlefieldEmptyLabel.Visible = boardCards.Count == 0;

		if (boardCards.Count > 0)
		{
			var grid = new GridContainer();
			grid.Columns = 6;
			_battlefieldContainer.AddChild(grid);

			foreach (var cardInstance in boardCards.ToList())
			{
				var cardNode = new CardNode();
				cardNode.Setup(cardInstance);
				grid.AddChild(cardNode);
			}
		}
	}

	private void OnPrevBoardPressed()
	{
		if (_context == null) return;
		_viewingPlayerIndex = (_viewingPlayerIndex - 1 + _context.Players.Count) % _context.Players.Count;
		UpdateBoardView();
	}

	private void OnNextBoardPressed()
	{
		if (_context == null) return;
		_viewingPlayerIndex = (_viewingPlayerIndex + 1) % _context.Players.Count;
		UpdateBoardView();
	}

	private void OnPassPriorityPressed()
	{
		if (_context != null && _context.PriorityPlayer == _context.Players[0])
		{
			_guiInput.OnPassPriorityPressed(_context.Players[0]);
		}
	}
}
