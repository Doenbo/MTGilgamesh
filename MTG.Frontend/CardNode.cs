using Godot;
using MTG.Engine.Gameplay;
using System;
using System.Threading.Tasks;

namespace MTG.Frontend;

public partial class CardNode : PanelContainer
{
    public CardInstance CardInstance { get; private set; }
    public Action<CardNode> OnCardClicked { get; set; }

    private StyleBoxFlat _panelStyle;
    private Label _nameLabel;
    private Label _typeLabel;
    private Label _manaLabel;
    private TextureRect _artTextureRect;
    private RichTextLabel _oracleLabel;
    private Label _ptLabel;
    private PanelContainer _ptContainer;

    private bool _isPlayable;
    private bool _isTapped;

    public override void _Ready()
    {
        CustomMinimumSize = new Vector2(165, 230);
        MouseFilter = MouseFilterEnum.Stop;

        // Base Styling for Card Frame
        _panelStyle = new StyleBoxFlat();
        _panelStyle.BgColor = new Color(0.1f, 0.12f, 0.16f, 0.96f);
        _panelStyle.BorderWidthBottom = 2;
        _panelStyle.BorderWidthLeft = 2;
        _panelStyle.BorderWidthRight = 2;
        _panelStyle.BorderWidthTop = 2;
        _panelStyle.BorderColor = new Color(0.8f, 0.65f, 0.25f); // Golden frame
        _panelStyle.CornerRadiusBottomLeft = 8;
        _panelStyle.CornerRadiusBottomRight = 8;
        _panelStyle.CornerRadiusTopLeft = 8;
        _panelStyle.CornerRadiusTopRight = 8;
        AddThemeStyleboxOverride("panel", _panelStyle);

        var marginContainer = new MarginContainer();
        marginContainer.AddThemeConstantOverride("margin_top", 6);
        marginContainer.AddThemeConstantOverride("margin_bottom", 6);
        marginContainer.AddThemeConstantOverride("margin_left", 6);
        marginContainer.AddThemeConstantOverride("margin_right", 6);
        AddChild(marginContainer);

        var vbox = new VBoxContainer();
        vbox.AddThemeConstantOverride("separation", 4);
        marginContainer.AddChild(vbox);

        // Header: Name & Mana Cost
        var headerHBox = new HBoxContainer();
        vbox.AddChild(headerHBox);

        _nameLabel = new Label();
        _nameLabel.SizeFlagsHorizontal = SizeFlags.ExpandFill;
        _nameLabel.AddThemeFontSizeOverride("font_size", 12);
        _nameLabel.Text = "Card Name";
        headerHBox.AddChild(_nameLabel);

        _manaLabel = new Label();
        _manaLabel.AddThemeFontSizeOverride("font_size", 11);
        _manaLabel.Modulate = new Color(1.0f, 0.85f, 0.4f);
        _manaLabel.Text = "";
        headerHBox.AddChild(_manaLabel);

        // Type Line Badge
        _typeLabel = new Label();
        _typeLabel.AddThemeFontSizeOverride("font_size", 10);
        _typeLabel.Modulate = new Color(0.75f, 0.85f, 0.95f);
        _typeLabel.Text = "Type Line";
        vbox.AddChild(_typeLabel);

        // Card Art TextureRect
        _artTextureRect = new TextureRect();
        _artTextureRect.CustomMinimumSize = new Vector2(0, 70);
        _artTextureRect.ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize;
        _artTextureRect.StretchMode = TextureRect.StretchModeEnum.KeepAspectCovered;
        _artTextureRect.Visible = false;
        vbox.AddChild(_artTextureRect);

        // Oracle Box
        _oracleLabel = new RichTextLabel();
        _oracleLabel.SizeFlagsVertical = SizeFlags.ExpandFill;
        _oracleLabel.BbcodeEnabled = true;
        _oracleLabel.ScrollFollowing = false;
        _oracleLabel.FocusMode = FocusModeEnum.None;
        _oracleLabel.MouseFilter = MouseFilterEnum.Ignore;
        _oracleLabel.AddThemeFontSizeOverride("normal_font_size", 10);
        vbox.AddChild(_oracleLabel);

        // P/T Container (Bottom Right)
        var footerHBox = new HBoxContainer();
        footerHBox.Alignment = BoxContainer.AlignmentMode.End;
        vbox.AddChild(footerHBox);

        _ptContainer = new PanelContainer();
        _ptContainer.Visible = false;

        var ptStyle = new StyleBoxFlat();
        ptStyle.BgColor = new Color(0.2f, 0.22f, 0.28f, 0.95f);
        ptStyle.BorderWidthBottom = 1; ptStyle.BorderWidthLeft = 1;
        ptStyle.BorderWidthRight = 1; ptStyle.BorderWidthTop = 1;
        ptStyle.BorderColor = new Color(0.9f, 0.8f, 0.3f);
        ptStyle.CornerRadiusBottomLeft = 4; ptStyle.CornerRadiusBottomRight = 4;
        ptStyle.CornerRadiusTopLeft = 4; ptStyle.CornerRadiusTopRight = 4;
        _ptContainer.AddThemeStyleboxOverride("panel", ptStyle);
        footerHBox.AddChild(_ptContainer);

        _ptLabel = new Label();
        _ptLabel.AddThemeFontSizeOverride("font_size", 11);
        _ptLabel.Modulate = new Color(1.0f, 0.95f, 0.6f);
        _ptLabel.Text = "";
        _ptContainer.AddChild(_ptLabel);

        GuiInput += OnGuiInput;
        MouseEntered += OnMouseEntered;
        MouseExited += OnMouseExited;

        if (CardInstance != null)
        {
            UpdateCardVisuals();
        }
    }

    public void Setup(CardInstance cardInstance, bool isPlayable = false, bool isTapped = false)
    {
        CardInstance = cardInstance;
        _isPlayable = isPlayable;
        _isTapped = isTapped;

        if (IsInsideTree())
        {
            UpdateCardVisuals();
        }
    }

    public void SetHighlight(bool isPlayable, bool isTapped)
    {
        _isPlayable = isPlayable;
        _isTapped = isTapped;
        UpdateCardFrameStyle();
    }

    private void UpdateCardVisuals()
    {
        if (CardInstance?.CardData == null) return;

        var card = CardInstance.CardData;
        _nameLabel.Text = card.FullName;
        _typeLabel.Text = card.FullTypeLine;

        // Oracle text
        _oracleLabel.Clear();
        _oracleLabel.AppendText($"[color=gainsboro]{card.ToString()}[/color]");

        // Tapped State
        _isTapped = CardInstance.IsTapped;
        Modulate = _isTapped ? new Color(0.6f, 0.6f, 0.6f) : new Color(1.0f, 1.0f, 1.0f);

        UpdateCardFrameStyle();

        // Async Texture Load
        _ = LoadArtAsync(card);
    }

    private async Task LoadArtAsync(MTG.Core.Cards.ICard card)
    {
        var texture = await CardImageLoader.LoadCardTextureAsync(card);
        if (texture != null && IsInsideTree() && CardInstance?.CardData == card)
        {
            _artTextureRect.Texture = texture;
            _artTextureRect.Visible = true;
        }
    }

    private void UpdateCardFrameStyle()
    {
        if (_panelStyle == null) return;

        if (_isPlayable)
        {
            // Glowing green border when valid to play
            _panelStyle.BorderColor = new Color(0.2f, 1.0f, 0.4f);
            _panelStyle.BorderWidthBottom = 3;
            _panelStyle.BorderWidthLeft = 3;
            _panelStyle.BorderWidthRight = 3;
            _panelStyle.BorderWidthTop = 3;
        }
        else
        {
            // Standard Golden Border
            _panelStyle.BorderColor = new Color(0.8f, 0.65f, 0.25f);
            _panelStyle.BorderWidthBottom = 2;
            _panelStyle.BorderWidthLeft = 2;
            _panelStyle.BorderWidthRight = 2;
            _panelStyle.BorderWidthTop = 2;
        }
    }

    private void OnGuiInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseBtn && mouseBtn.Pressed && mouseBtn.ButtonIndex == MouseButton.Left)
        {
            OnCardClicked?.Invoke(this);
        }
    }

    private void OnMouseEntered()
    {
        Scale = new Vector2(1.05f, 1.05f);
        PivotOffset = CustomMinimumSize / 2.0f;
        Main.Instance?.ShowCardPreview(CardInstance);
    }

    private void OnMouseExited()
    {
        Scale = new Vector2(1.0f, 1.0f);
        Main.Instance?.HideCardPreview();
    }
}
