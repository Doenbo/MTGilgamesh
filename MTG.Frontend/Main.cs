using Godot;
using Microsoft.Extensions.Logging;
using MTG.Core.Enums;
using MTG.Core.Helper;
using MTG.Engine.Cards;
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

	// 4-Player Top Dashboard Widgets (Human + 3 Bots)
	private readonly List<PanelContainer> _playerWidgets = [];
	private readonly List<Label> _playerNameLabels = [];
	private readonly List<Label> _playerLifeLabels = [];
	private readonly List<Label> _playerHandLabels = [];

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

	// Interactive Targeting & Game Over Overlay
	private TargetingLine _targetingLine;
	private PanelContainer _gameOverOverlay;
	private Label _gameOverTitleLabel;
	private Label _gameOverDescLabel;

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

		_targetingLine = new TargetingLine();
		canvasLayer.AddChild(_targetingLine);

		// Match End GameOver Overlay Modal
		_gameOverOverlay = new PanelContainer();
		_gameOverOverlay.SetAnchorsPreset(Control.LayoutPreset.FullRect);
		_gameOverOverlay.AnchorRight = 1.0f;
		_gameOverOverlay.AnchorBottom = 1.0f;
		_gameOverOverlay.Visible = false;

		var overlayStyle = new StyleBoxFlat();
		overlayStyle.BgColor = new Color(0.02f, 0.04f, 0.08f, 0.88f);
		_gameOverOverlay.AddThemeStyleboxOverride("panel", overlayStyle);
		canvasLayer.AddChild(_gameOverOverlay);

		var gameOverVBox = new VBoxContainer();
		gameOverVBox.Alignment = BoxContainer.AlignmentMode.Center;
		_gameOverOverlay.AddChild(gameOverVBox);

		_gameOverTitleLabel = new Label();
		_gameOverTitleLabel.Text = "VICTORY!";
		_gameOverTitleLabel.AddThemeFontSizeOverride("font_size", 48);
		_gameOverTitleLabel.HorizontalAlignment = HorizontalAlignment.Center;
		_gameOverTitleLabel.Modulate = new Color(1.0f, 0.85f, 0.2f);
		gameOverVBox.AddChild(_gameOverTitleLabel);

		_gameOverDescLabel = new Label();
		_gameOverDescLabel.Text = "All bot opponents have been defeated!";
		_gameOverDescLabel.AddThemeFontSizeOverride("font_size", 18);
		_gameOverDescLabel.HorizontalAlignment = HorizontalAlignment.Center;
		gameOverVBox.AddChild(_gameOverDescLabel);

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

		// 4-Player Top Dashboard Widgets (Human + 3 Bots)
		var playersDashboardHBox = new HBoxContainer();
		playersDashboardHBox.AddThemeConstantOverride("separation", 8);
		topBarHBox.AddChild(playersDashboardHBox);

		for (int i = 0; i < 4; i++)
		{
			int playerIndex = i;
			var playerWidget = new PanelContainer
			{
				CustomMinimumSize = new Vector2(130, 42)
			};

			var playerStyle = new StyleBoxFlat
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
			playerWidget.AddThemeStyleboxOverride("panel", playerStyle);

			var playerVBox = new VBoxContainer();
			playerVBox.AddThemeConstantOverride("separation", 1);
			playerWidget.AddChild(playerVBox);

			var nameLabel = new Label
			{
				Text = playerIndex == 0 ? "You" : $"Bot {playerIndex}"
			};
			nameLabel.AddThemeFontSizeOverride("font_size", 11);
			nameLabel.HorizontalAlignment = HorizontalAlignment.Center;
			playerVBox.AddChild(nameLabel);

			var statsHBox = new HBoxContainer
			{
				Alignment = BoxContainer.AlignmentMode.Center
			};
			playerVBox.AddChild(statsHBox);

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

			_playerWidgets.Add(playerWidget);
			_playerNameLabels.Add(nameLabel);
			_playerLifeLabels.Add(lifeLabel);
			_playerHandLabels.Add(handLabel);

			// 1-Click Board Switch to Player
			playerWidget.GuiInput += (@evt) =>
			{
				if (@evt is InputEventMouseButton mb && mb.Pressed && mb.ButtonIndex == MouseButton.Left)
				{
					_viewingPlayerIndex = playerIndex;
					UpdateBoardView();
				}
			};

			playersDashboardHBox.AddChild(playerWidget);
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

		// Card Inspector Preview Panel (Left Overlay Panel - Always Visible)
		_previewPanel = new PanelContainer
		{
			CustomMinimumSize = new Vector2(210, 0),
			Visible = true
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
		var boardPanel = new BattlefieldDropZone
		{
			SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
			SizeFlagsVertical = Control.SizeFlags.ExpandFill,
			SizeFlagsStretchRatio = 3.0f
		};
		boardPanel.OnCardDropped = OnCardDroppedOnBattlefield;

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

		UpdatePlayersDashboard();
		UpdateBoardView();
		UpdateHumanHand();

		GameLog.AppendText("[color=green]=== 4-Player Match Started! ===[/color]\n");

		await Task.Run(() => engine.StartGameLoop());
	}

	public void SetPriorityPrompt(CommanderPlayer priorityPlayer, TurnStep step, bool isStackActive)
	{
		CheckGameOverState();
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

		UpdatePlayersDashboard();
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
		// Panel stays permanently visible on the left to avoid annoying layout shifting
	}

	private void UpdatePlayersDashboard()
	{
		if (_context == null || _context.Players.Count < 4) return;

		for (int i = 0; i < 4; i++)
		{
			var player = _context.Players[i];
			var widget = _playerWidgets[i];

			_playerNameLabels[i].Text = i == 0 ? $"{player.Name} (You)" : player.Name;
			_playerLifeLabels[i].Text = $"❤ {player.LifeTotal}";
			_playerHandLabels[i].Text = $" | 🂠 {player.Hand.Count}";

			// Active Turn / Priority Glow & Viewing Board Highlight
			bool isPlayerActive = _context.PriorityPlayer == player;
			bool isViewingThisBoard = _viewingPlayerIndex == i;

			var style = widget.GetThemeStylebox("panel") as StyleBoxFlat;
			if (style != null)
			{
				if (isPlayerActive)
				{
					style.BorderColor = new Color(0.3f, 1.0f, 0.4f); // Glowing Green
					style.BorderWidthBottom = 2; style.BorderWidthTop = 2;
					style.BorderWidthLeft = 2; style.BorderWidthRight = 2;
				}
				else if (isViewingThisBoard)
				{
					style.BorderColor = new Color(0.9f, 0.8f, 0.3f); // Golden Accent
					style.BorderWidthBottom = 2; style.BorderWidthTop = 2;
					style.BorderWidthLeft = 2; style.BorderWidthRight = 2;
				}
				else
				{
					style.BorderColor = new Color(0.3f, 0.4f, 0.5f);
					style.BorderWidthBottom = 1; style.BorderWidthTop = 1;
					style.BorderWidthLeft = 1; style.BorderWidthRight = 1;
				}
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

		foreach (var cardInstance in human.Hand.Cards.ToList())
		{
			var cardNode = new CardNode();
			cardNode.Setup(cardInstance, isPlayable: isHumanTurn);
			// Drag & Drop only: cards in hand must be dragged onto the battlefield panel to be cast
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

	public void CheckGameOverState()
	{
		if (_context == null || _context.Players.Count < 4) return;

		var human = _context.Players[0];
		if (human.LifeTotal <= 0)
		{
			ShowGameOver(isVictory: false, "Your life total reached 0 HP.");
			return;
		}

		bool allBotsDefeated = _context.Players.Skip(1).All(b => b.LifeTotal <= 0);
		if (allBotsDefeated)
		{
			ShowGameOver(isVictory: true, "All bot opponents have been eliminated!");
		}
	}

	private void ShowGameOver(bool isVictory, string details)
	{
		_gameOverOverlay.Visible = true;
		_gameOverTitleLabel.Text = isVictory ? "🏆 VICTORY!" : "💀 DEFEAT";
		_gameOverTitleLabel.Modulate = isVictory ? new Color(1.0f, 0.85f, 0.2f) : new Color(1.0f, 0.3f, 0.3f);
		_gameOverDescLabel.Text = details;
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event is InputEventKey keyEv && keyEv.Pressed && !keyEv.Echo)
		{
			if (keyEv.Keycode == Key.Left || keyEv.Keycode == Key.A)
			{
				OnPrevBoardPressed();
			}
			else if (keyEv.Keycode == Key.Right || keyEv.Keycode == Key.D)
			{
				OnNextBoardPressed();
			}
		}
	}

	private void OnCardDroppedOnBattlefield(CardNode cardNode)
	{
		if (_context != null && cardNode?.CardInstance != null)
		{
			var human = _context.Players[0];
			if (_context.PriorityPlayer == human)
			{
				_guiInput?.OnCardClicked(human, cardNode.CardInstance);
			}
		}
	}
}
