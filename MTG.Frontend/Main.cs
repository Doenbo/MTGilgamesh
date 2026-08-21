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
	public static Main Instance { get; private set; }

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
	private readonly List<PanelContainer> _botWidgets = [];
	private readonly List<Label> _botLifeLabels = [];
	private readonly List<Label> _botHandLabels = [];

	// Fullscreen Board UI
	private VBoxContainer _battlefieldContainer;
	private Label _battlefieldEmptyLabel;

	// Center Stack Overlay Window
	private PanelContainer _stackPanel;
	private VBoxContainer _stackListVBox;

	// Card Inspector Preview Panel
	private PanelContainer _previewPanel;
	private Label _previewTitleLabel;
	private Label _previewTypeLabel;
	private TextureRect _previewTextureRect;
	private RichTextLabel _previewOracleLabel;

	// Bottom Human HUD
	private Label _humanLifeLabel;
	private Label _humanManaLabel;
	private HBoxContainer _handContainer;
	private Button _passPriorityBtn;

	// Phase Tracker Bar Labels
	private readonly Dictionary<TurnStep, Label> _phasePillLabels = [];

	private ILoggerFactory _loggerFactory;

	public override void _Ready()
	{
		Instance = this;
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
        // 1. TOP BAR: Interactive Phase Tracker + 3-Bot Dashboard + Board Switcher Arrows
        // ==========================================
        var topBarHBox = new HBoxContainer
        {
            CustomMinimumSize = new Vector2(0, 50),
            SizeFlagsHorizontal = Control.SizeFlags.ExpandFill
        };
        mainVBox.AddChild(topBarHBox);

		// Visual Phase Tracker Bar
		var phaseTrackerHBox = new HBoxContainer();
		phaseTrackerHBox.AddThemeConstantOverride("separation", 4);
		phaseTrackerHBox.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
		topBarHBox.AddChild(phaseTrackerHBox);

		TurnStep[] stepsToTrack = { TurnStep.Untap, TurnStep.Upkeep, TurnStep.Draw, TurnStep.Main1, TurnStep.CombatBegin, TurnStep.Main2, TurnStep.EndStep };
		foreach (var step in stepsToTrack)
		{
            var pill = new Label
            {
                Text = $" {step} "
            };
            pill.AddThemeFontSizeOverride("font_size", 10);
			pill.Modulate = new Color(0.6f, 0.6f, 0.6f);
			phaseTrackerHBox.AddChild(pill);
			_phasePillLabels[step] = pill;
		}

		// 3-Bot Opponent Dashboard Widgets
		var botsDashboardHBox = new HBoxContainer();
		botsDashboardHBox.AddThemeConstantOverride("separation", 8);
		topBarHBox.AddChild(botsDashboardHBox);

		for (int i = 1; i <= 3; i++)
		{
			int botIndex = i;
            var botWidget = new PanelContainer
            {
                CustomMinimumSize = new Vector2(130, 42)
            };

            var botStyle = new StyleBoxFlat
            {
                BgColor = new Color(0.12f, 0.15f, 0.2f, 0.95f),
                BorderWidthBottom = 1,
                BorderWidthLeft = 1,
                BorderWidthRight = 1,
                BorderWidthTop = 1,
                BorderColor = new Color(0.3f, 0.4f, 0.5f),
                CornerRadiusBottomLeft = 6,
                CornerRadiusBottomRight = 6,
                CornerRadiusTopLeft = 6,
                CornerRadiusTopRight = 6
            };
            botWidget.AddThemeStyleboxOverride("panel", botStyle);

			var botVBox = new VBoxContainer();
			botVBox.AddThemeConstantOverride("separation", 1);
			botWidget.AddChild(botVBox);

            var nameLabel = new Label
            {
                Text = $"Bot {botIndex}"
            };
            nameLabel.AddThemeFontSizeOverride("font_size", 11);
			nameLabel.HorizontalAlignment = HorizontalAlignment.Center;
			botVBox.AddChild(nameLabel);

            var statsHBox = new HBoxContainer
            {
                Alignment = BoxContainer.AlignmentMode.Center
            };
            botVBox.AddChild(statsHBox);

            var lifeLabel = new Label
            {
                Text = "❤ 40"
            };
            lifeLabel.AddThemeFontSizeOverride("font_size", 10);
			lifeLabel.Modulate = new Color(0.3f, 1.0f, 0.4f);
			statsHBox.AddChild(lifeLabel);

            var handLabel = new Label
            {
                Text = " | 🂠 7"
            };
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

        _prevBoardBtn = new Button
        {
            Text = " ◄ ",
            CustomMinimumSize = new Vector2(36, 36)
        };
        _prevBoardBtn.Pressed += OnPrevBoardPressed;
		boardNavHBox.AddChild(_prevBoardBtn);

        _boardTitleLabel = new Label
        {
            Text = " BOARD: Dön (You) "
        };
        _boardTitleLabel.AddThemeFontSizeOverride("font_size", 14);
		_boardTitleLabel.VerticalAlignment = VerticalAlignment.Center;
		boardNavHBox.AddChild(_boardTitleLabel);

        _nextBoardBtn = new Button
        {
            Text = " ► ",
            CustomMinimumSize = new Vector2(36, 36)
        };
        _nextBoardBtn.Pressed += OnNextBoardPressed;
		boardNavHBox.AddChild(_nextBoardBtn);

        // ==========================================
        // 2. MIDDLE AREA: Fullscreen Active Board View + Inspector Preview + Stack Overlay + Logs
        // ==========================================
        var middleHSplit = new HBoxContainer
        {
            SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
            SizeFlagsVertical = Control.SizeFlags.ExpandFill,
            SizeFlagsStretchRatio = 7.0f
        };
        mainVBox.AddChild(middleHSplit);

        // Card Inspector Preview Panel (Left Overlay Panel)
        _previewPanel = new PanelContainer
        {
            CustomMinimumSize = new Vector2(200, 0),
            Visible = false
        };

        var previewStyle = new StyleBoxFlat
        {
            BgColor = new Color(0.08f, 0.1f, 0.14f, 0.98f),
            BorderWidthBottom = 2,
            BorderWidthLeft = 2,
            BorderWidthRight = 2,
            BorderWidthTop = 2,
            BorderColor = new Color(0.8f, 0.7f, 0.3f),
            CornerRadiusBottomLeft = 6,
            CornerRadiusBottomRight = 6,
            CornerRadiusTopLeft = 6,
            CornerRadiusTopRight = 6
        };
        _previewPanel.AddThemeStyleboxOverride("panel", previewStyle);
		middleHSplit.AddChild(_previewPanel);

		var previewVBox = new VBoxContainer();
		_previewPanel.AddChild(previewVBox);

        _previewTitleLabel = new Label
        {
            Text = "Card Preview"
        };
        _previewTitleLabel.AddThemeFontSizeOverride("font_size", 13);
		_previewTitleLabel.Modulate = new Color(1.0f, 0.9f, 0.5f);
		previewVBox.AddChild(_previewTitleLabel);

        _previewTypeLabel = new Label
        {
            Text = "Type Line"
        };
        _previewTypeLabel.AddThemeFontSizeOverride("font_size", 10);
		_previewTypeLabel.Modulate = new Color(0.8f, 0.85f, 0.95f);
		previewVBox.AddChild(_previewTypeLabel);

        _previewTextureRect = new TextureRect
        {
            CustomMinimumSize = new Vector2(0, 140),
            ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
            StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered,
            Visible = false
        };
        previewVBox.AddChild(_previewTextureRect);

        _previewOracleLabel = new RichTextLabel
        {
            SizeFlagsVertical = Control.SizeFlags.ExpandFill,
            BbcodeEnabled = true,
            FocusMode = Control.FocusModeEnum.None,
            MouseFilter = Control.MouseFilterEnum.Ignore
        };
        _previewOracleLabel.AddThemeFontSizeOverride("normal_font_size", 10);
		previewVBox.AddChild(_previewOracleLabel);

        // Fullscreen Board Panel (Center 70% width)
        var boardPanel = new PanelContainer
        {
            SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
            SizeFlagsVertical = Control.SizeFlags.ExpandFill,
            SizeFlagsStretchRatio = 3.0f
        };

        var boardStyle = new StyleBoxFlat
        {
            BgColor = new Color(0.08f, 0.1f, 0.13f, 0.95f),
            BorderWidthBottom = 2,
            BorderWidthLeft = 2,
            BorderWidthRight = 2,
            BorderWidthTop = 2,
            BorderColor = new Color(0.25f, 0.35f, 0.45f)
        };
        boardPanel.AddThemeStyleboxOverride("panel", boardStyle);
		middleHSplit.AddChild(boardPanel);

		var boardVBox = new VBoxContainer();
		boardPanel.AddChild(boardVBox);

        _battlefieldEmptyLabel = new Label
        {
            Text = "\n\n   (Battlefield is currently empty)",
            HorizontalAlignment = HorizontalAlignment.Center
        };
        _battlefieldEmptyLabel.AddThemeFontSizeOverride("font_size", 14);
		_battlefieldEmptyLabel.Modulate = new Color(0.6f, 0.6f, 0.6f);
		boardVBox.AddChild(_battlefieldEmptyLabel);

        _battlefieldContainer = new VBoxContainer
        {
            SizeFlagsVertical = Control.SizeFlags.ExpandFill
        };
        boardVBox.AddChild(_battlefieldContainer);

        // Center Stack Window Overlay
        _stackPanel = new PanelContainer
        {
            CustomMinimumSize = new Vector2(210, 130),
            Visible = false
        };

        var stackStyle = new StyleBoxFlat
        {
            BgColor = new Color(0.18f, 0.12f, 0.25f, 0.95f),
            BorderWidthBottom = 2,
            BorderWidthLeft = 2,
            BorderWidthRight = 2,
            BorderWidthTop = 2,
            BorderColor = new Color(0.8f, 0.3f, 0.9f) // Magenta stack border
        };
        _stackPanel.AddThemeStyleboxOverride("panel", stackStyle);
		middleHSplit.AddChild(_stackPanel);

		var stackVBox = new VBoxContainer();
		_stackPanel.AddChild(stackVBox);

        var stackTitle = new Label
        {
            Text = "⚡ THE STACK ⚡"
        };
        stackTitle.AddThemeFontSizeOverride("font_size", 12);
		stackTitle.HorizontalAlignment = HorizontalAlignment.Center;
		stackTitle.Modulate = new Color(0.9f, 0.4f, 1.0f);
		stackVBox.AddChild(stackTitle);

		_stackListVBox = new VBoxContainer();
		stackVBox.AddChild(_stackListVBox);

        // Right Log Panel (Right 25% width)
        var logVBox = new VBoxContainer
        {
            SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
            SizeFlagsVertical = Control.SizeFlags.ExpandFill,
            SizeFlagsStretchRatio = 1.0f
        };
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
        var hudPanel = new PanelContainer
        {
            CustomMinimumSize = new Vector2(0, 185),
            SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
            SizeFlagsVertical = Control.SizeFlags.ShrinkEnd
        };

        var hudStyle = new StyleBoxFlat
        {
            BgColor = new Color(0.05f, 0.07f, 0.1f, 0.98f),
            BorderWidthTop = 3,
            BorderColor = new Color(0.8f, 0.65f, 0.2f) // Golden accent
        };
        hudPanel.AddThemeStyleboxOverride("panel", hudStyle);
		mainVBox.AddChild(hudPanel);

		var hudHBox = new HBoxContainer();
		hudHBox.AddThemeConstantOverride("separation", 12);
		hudPanel.AddChild(hudHBox);

        // Player Stats & Mana Pool Dashboard Box
        var statsVBox = new VBoxContainer
        {
            CustomMinimumSize = new Vector2(170, 0),
            Alignment = BoxContainer.AlignmentMode.Center
        };
        hudHBox.AddChild(statsVBox);

        _humanLifeLabel = new Label
        {
            Text = "❤ Dön: 40 HP"
        };
        _humanLifeLabel.AddThemeFontSizeOverride("font_size", 16);
		_humanLifeLabel.Modulate = new Color(0.3f, 1.0f, 0.4f);
		statsVBox.AddChild(_humanLifeLabel);

        _humanManaLabel = new Label
        {
            Text = "Pool: W:0 U:0 B:0 R:0 G:0 C:0"
        };
        _humanManaLabel.AddThemeFontSizeOverride("font_size", 11);
		_humanManaLabel.Modulate = new Color(0.9f, 0.85f, 0.5f);
		statsVBox.AddChild(_humanManaLabel);

        // Scrollable Hand Container
        var handScroll = new ScrollContainer
        {
            SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
            SizeFlagsVertical = Control.SizeFlags.ExpandFill
        };
        hudHBox.AddChild(handScroll);

		_handContainer = new HBoxContainer();
		_handContainer.AddThemeConstantOverride("separation", 8);
		handScroll.AddChild(_handContainer);

        // Action Bar
        var actionVBox = new VBoxContainer
        {
            CustomMinimumSize = new Vector2(165, 0),
            Alignment = BoxContainer.AlignmentMode.Center
        };
        hudHBox.AddChild(actionVBox);

        _passPriorityBtn = new Button
        {
            Text = "END PHASE",
            CustomMinimumSize = new Vector2(155, 50)
        };
        _passPriorityBtn.AddThemeFontSizeOverride("font_size", 13);
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
		UpdatePhaseTracker(step);

		bool isHumanTurn = priorityPlayer == _context.Players[0];
		_passPriorityBtn.Disabled = !isHumanTurn;

		if (isHumanTurn)
		{
			if (isStackActive)
			{
				_passPriorityBtn.Text = "RESOLVE (PASS)";
				_passPriorityBtn.Modulate = new Color(0.9f, 0.3f, 1.0f); // Magenta
			}
			else
			{
				_passPriorityBtn.Text = "END PHASE";
				_passPriorityBtn.Modulate = new Color(0.3f, 1.0f, 0.4f); // Green
			}
		}
		else
		{
			_passPriorityBtn.Text = $"WAITING FOR {priorityPlayer.Name.ToUpper()}...";
			_passPriorityBtn.Modulate = new Color(0.6f, 0.6f, 0.6f); // Disabled Gray
		}

		UpdateOpponentsDashboard();
		UpdateStackOverlay();
		UpdateHumanHand();
		UpdateBoardView();
	}

	public void ClearPriorityPrompt()
	{
		_passPriorityBtn.Disabled = true;
		_passPriorityBtn.Text = "PROCESSING...";
	}

	private void UpdatePhaseTracker(TurnStep currentStep)
	{
		foreach (var kvp in _phasePillLabels)
		{
			if (kvp.Key == currentStep)
			{
				kvp.Value.Modulate = new Color(1.0f, 0.85f, 0.3f); // Highlight active phase
				kvp.Value.Text = $" ►[{kvp.Key}]◄ ";
			}
			else
			{
				kvp.Value.Modulate = new Color(0.5f, 0.5f, 0.5f);
				kvp.Value.Text = $" {kvp.Key} ";
			}
		}
	}

	public async void ShowCardPreview(CardInstance cardInstance)
	{
		if (cardInstance?.CardData == null) return;
		var card = cardInstance.CardData;

		_previewPanel.Visible = true;
		_previewTitleLabel.Text = card.FullName;
		_previewTypeLabel.Text = card.FullTypeLine;

		_previewOracleLabel.Clear();
		_previewOracleLabel.AppendText($"[color=gainsboro]{card.ToString()}[/color]");

		var texture = await CardImageLoader.LoadCardTextureAsync(card);
		if (texture != null)
		{
			_previewTextureRect.Texture = texture;
			_previewTextureRect.Visible = true;
		}
		else
		{
			_previewTextureRect.Visible = false;
		}
	}

	public void HideCardPreview()
	{
		_previewPanel.Visible = false;
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
                var lbl = new Label
                {
                    Text = $"• {card.CardData.FullName} ({card.Owner.Name})"
                };
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
            var grid = new GridContainer
            {
                Columns = 6
            };
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
