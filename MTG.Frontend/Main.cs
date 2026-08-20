using Godot;
using Microsoft.Extensions.Logging;
using MTG.Core.Enums;
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
	private int _viewingPlayerIndex = 0; // 0 = Human (Dön), 1 = Bot 1 (Nüs), 2 = Bot 2 (Zag), 3 = Bot 3 (Mel)
	private Label _boardTitleLabel;
	private Button _prevBoardBtn;
	private Button _nextBoardBtn;

	// 3-Bot Opponent Dashboard Widgets
	private readonly List<PanelContainer> _botWidgets = new();
	private readonly List<Label> _botLifeLabels = new();
	private readonly List<Label> _botHandLabels = new();

	// Fullscreen Board UI
	private VBoxContainer _battlefieldContainer;
	private Label _battlefieldEmptyLabel;

	// Center Stack Overlay Window
	private PanelContainer _stackPanel;
	private VBoxContainer _stackListVBox;

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
		// 1. TOP BAR: Turn Banner + 3-Bot Dashboard + Board Switcher Arrows
		// ==========================================
		var topBarHBox = new HBoxContainer();
		topBarHBox.CustomMinimumSize = new Vector2(0, 50);
		topBarHBox.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
		mainVBox.AddChild(topBarHBox);

		// Step & Priority Banner
		_stepBannerLabel = new Label();
		_stepBannerLabel.Text = " [Untap Step] ";
		_stepBannerLabel.AddThemeFontSizeOverride("font_size", 15);
		_stepBannerLabel.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
		_stepBannerLabel.VerticalAlignment = VerticalAlignment.Center;
		topBarHBox.AddChild(_stepBannerLabel);

		// 3-Bot Opponent Dashboard Widgets
		var botsDashboardHBox = new HBoxContainer();
		botsDashboardHBox.AddThemeConstantOverride("separation", 8);
		topBarHBox.AddChild(botsDashboardHBox);

		for (int i = 1; i <= 3; i++)
		{
			int botIndex = i;
			var botWidget = new PanelContainer();
			botWidget.CustomMinimumSize = new Vector2(140, 42);

			var botStyle = new StyleBoxFlat();
			botStyle.BgColor = new Color(0.12f, 0.15f, 0.2f, 0.95f);
			botStyle.BorderWidthBottom = 1; botStyle.BorderWidthLeft = 1;
			botStyle.BorderWidthRight = 1; botStyle.BorderWidthTop = 1;
			botStyle.BorderColor = new Color(0.3f, 0.4f, 0.5f);
			botStyle.CornerRadiusBottomLeft = 6; botStyle.CornerRadiusBottomRight = 6;
			botStyle.CornerRadiusTopLeft = 6; botStyle.CornerRadiusTopRight = 6;
			botWidget.AddThemeStyleboxOverride("panel", botStyle);

			var botVBox = new VBoxContainer();
			botVBox.AddThemeConstantOverride("separation", 1);
			botWidget.AddChild(botVBox);

			var nameLabel = new Label();
			nameLabel.Text = $"Bot {botIndex}";
			nameLabel.AddThemeFontSizeOverride("font_size", 11);
			nameLabel.HorizontalAlignment = HorizontalAlignment.Center;
			botVBox.AddChild(nameLabel);

			var statsHBox = new HBoxContainer();
			statsHBox.Alignment = BoxContainer.AlignmentMode.Center;
			botVBox.AddChild(statsHBox);

			var lifeLabel = new Label();
			lifeLabel.Text = "❤ 40";
			lifeLabel.AddThemeFontSizeOverride("font_size", 10);
			lifeLabel.Modulate = new Color(0.3f, 1.0f, 0.4f);
			statsHBox.AddChild(lifeLabel);

			var handLabel = new Label();
			handLabel.Text = " | 🂠 7";
			handLabel.AddThemeFontSizeOverride("font_size", 10);
			statsHBox.AddChild(handLabel);

			_botWidgets.Add(botWidget);
			_botLifeLabels.Add(lifeLabel);
			_botHandLabels.Add(handLabel);

			// 1-Click Board Switch to Bot
			botWidget.GuiInput += (@evt) =>
			{
				if (@evt is InputEventMouseButton mb && mb.Pressed && mb.ButtonIndex == MouseButton.Left)
				{
					_viewingPlayerIndex = botIndex;
					UpdateBoardView();
				}
			};

			botsDashboardHBox.AddChild(botWidget);
		}

		// Top-Right Board Switcher Arrows Controls
		var boardNavHBox = new HBoxContainer();
		boardNavHBox.AddThemeConstantOverride("separation", 4);
		topBarHBox.AddChild(boardNavHBox);

		_prevBoardBtn = new Button();
		_prevBoardBtn.Text = " ◄ ";
		_prevBoardBtn.CustomMinimumSize = new Vector2(36, 36);
		_prevBoardBtn.Pressed += OnPrevBoardPressed;
		boardNavHBox.AddChild(_prevBoardBtn);

		_boardTitleLabel = new Label();
		_boardTitleLabel.Text = " BOARD: Dön (You) ";
		_boardTitleLabel.AddThemeFontSizeOverride("font_size", 14);
		_boardTitleLabel.VerticalAlignment = VerticalAlignment.Center;
		boardNavHBox.AddChild(_boardTitleLabel);

		_nextBoardBtn = new Button();
		_nextBoardBtn.Text = " ► ";
		_nextBoardBtn.CustomMinimumSize = new Vector2(36, 36);
		_nextBoardBtn.Pressed += OnNextBoardPressed;
		boardNavHBox.AddChild(_nextBoardBtn);

		// ==========================================
		// 2. MIDDLE AREA: Fullscreen Active Board View + Stack Overlay + Logs
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

		// Center Stack Window Overlay
		_stackPanel = new PanelContainer();
		_stackPanel.CustomMinimumSize = new Vector2(220, 140);
		_stackPanel.Visible = false;

		var stackStyle = new StyleBoxFlat();
		stackStyle.BgColor = new Color(0.18f, 0.12f, 0.25f, 0.95f);
		stackStyle.BorderWidthBottom = 2; stackStyle.BorderWidthLeft = 2;
		stackStyle.BorderWidthRight = 2; stackStyle.BorderWidthTop = 2;
		stackStyle.BorderColor = new Color(0.8f, 0.3f, 0.9f); // Magenta stack border
		_stackPanel.AddThemeStyleboxOverride("panel", stackStyle);
		middleHSplit.AddChild(_stackPanel);

		var stackVBox = new VBoxContainer();
		_stackPanel.AddChild(stackVBox);

		var stackTitle = new Label();
		stackTitle.Text = "⚡ THE STACK ⚡";
		stackTitle.AddThemeFontSizeOverride("font_size", 12);
		stackTitle.HorizontalAlignment = HorizontalAlignment.Center;
		stackTitle.Modulate = new Color(0.9f, 0.4f, 1.0f);
		stackVBox.AddChild(stackTitle);

		_stackListVBox = new VBoxContainer();
		stackVBox.AddChild(_stackListVBox);

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
		DevLog.Visible = false;
		logVBox.AddChild(DevLog);

		// ==========================================
		// 3. BOTTOM HUD: Human Player Hand & Priority Control Bar
		// ==========================================
		var hudPanel = new PanelContainer();
		hudPanel.CustomMinimumSize = new Vector2(0, 185);
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

		// Player Stats & Mana Pool Dashboard Box
		var statsVBox = new VBoxContainer();
		statsVBox.CustomMinimumSize = new Vector2(170, 0);
		statsVBox.Alignment = BoxContainer.AlignmentMode.Center;
		hudHBox.AddChild(statsVBox);

		_humanLifeLabel = new Label();
		_humanLifeLabel.Text = "❤ Dön: 40 HP";
		_humanLifeLabel.AddThemeFontSizeOverride("font_size", 16);
		_humanLifeLabel.Modulate = new Color(0.3f, 1.0f, 0.4f);
		statsVBox.AddChild(_humanLifeLabel);

		_humanManaLabel = new Label();
		_humanManaLabel.Text = "Pool: W:0 U:0 B:0 R:0 G:0 C:0";
		_humanManaLabel.AddThemeFontSizeOverride("font_size", 11);
		_humanManaLabel.Modulate = new Color(0.9f, 0.85f, 0.5f);
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

		UpdateOpponentsDashboard();
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

		UpdateOpponentsDashboard();
		UpdateStackOverlay();
		UpdateHumanHand();
		UpdateBoardView();
	}

	public void ClearPriorityPrompt()
	{
		_passPriorityBtn.Disabled = true;
	}

	private void UpdateOpponentsDashboard()
	{
		if (_context == null || _context.Players.Count < 4) return;

		for (int i = 1; i <= 3; i++)
		{
			var bot = _context.Players[i];
			var widget = _botWidgets[i - 1];
			_botLifeLabels[i - 1].Text = $"❤ {bot.LifeTotal}";
			_botHandLabels[i - 1].Text = $" | 🂠 {bot.Hand.Count}";

			// Active Turn / Priority Glow
			bool isBotActive = _context.PriorityPlayer == bot;
			var style = widget.GetThemeStylebox("panel") as StyleBoxFlat;
			if (style != null)
			{
				style.BorderColor = isBotActive ? new Color(0.3f, 1.0f, 0.4f) : new Color(0.3f, 0.4f, 0.5f);
				style.BorderWidthBottom = isBotActive ? 2 : 1;
				style.BorderWidthTop = isBotActive ? 2 : 1;
			}
		}
	}

	private void UpdateStackOverlay()
	{
		if (_context == null || _stackListVBox == null) return;

		foreach (var child in _stackListVBox.GetChildren().ToList())
		{
			child.QueueFree();
		}

		bool hasStack = _context.StackCount > 0;
		_stackPanel.Visible = hasStack;

		if (hasStack)
		{
			foreach (var card in _context.Stack.ToList())
			{
				var lbl = new Label();
				lbl.Text = $"• {card.CardData.FullName} ({card.Owner.Name})";
				lbl.AddThemeFontSizeOverride("font_size", 10);
				_stackListVBox.AddChild(lbl);
			}
		}
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

		var pool = human.ManaPool;
		if (pool != null)
		{
			int w = pool.Mana.Count(m => m.IsFixed && m.ManaFixed == ManaType.White);
			int u = pool.Mana.Count(m => m.IsFixed && m.ManaFixed == ManaType.Blue);
			int b = pool.Mana.Count(m => m.IsFixed && m.ManaFixed == ManaType.Black);
			int r = pool.Mana.Count(m => m.IsFixed && m.ManaFixed == ManaType.Red);
			int g = pool.Mana.Count(m => m.IsFixed && m.ManaFixed == ManaType.Green);
			int c = pool.Mana.Count(m => m.IsFixed && m.ManaFixed == ManaType.Colorless);

			_humanManaLabel.Text = $"Pool ({pool.TotalMana}): W:{w} U:{u} B:{b} R:{r} G:{g} C:{c}";
		}

		bool isHumanTurn = _context.PriorityPlayer == human;

		foreach (var cardInstance in human.Hand.ToList())
		{
			var cardNode = new CardNode();
			cardNode.Setup(cardInstance, isPlayable: isHumanTurn);
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
				cardNode.Setup(cardInstance, isPlayable: false, isTapped: cardInstance.IsTapped);
				cardNode.OnCardClicked = (node) =>
				{
					// Interactive Land Tapping on Human Battlefield
					if (_viewingPlayerIndex == 0 && targetPlayer == _context.Players[0] && !cardInstance.IsTapped && cardInstance.CardData.IsLand())
					{
						cardInstance.IsTapped = true;
						var mu = MTG.Core.Abilities.ManaUnit.CreateFixed(ManaType.Colorless);
						if (mu.IsSuccess)
						{
							targetPlayer.ManaPool?.AddMana(mu.Value);
						}
						UpdateHumanHand();
						UpdateBoardView();
					}
				};
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
