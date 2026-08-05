using Godot;
using MTG.Engine.Gameplay;
using System;

namespace MTG.Frontend;

public partial class CardNode : PanelContainer
{
    public CardInstance CardInstance { get; private set; }
    public Action<CardNode> OnCardClicked { get; set; }

    private Label _nameLabel;
    private Label _typeLabel;
    private Label _manaLabel;
    private RichTextLabel _oracleLabel;
    private Label _ptLabel;

    public override void _Ready()
    {
        CustomMinimumSize = new Vector2(160, 220);
        MouseFilter = MouseFilterEnum.Stop;

        // Visual Styling for Card Frame
        var style = new StyleBoxFlat();
        style.BgColor = new Color(0.12f, 0.14f, 0.18f, 0.95f);
        style.BorderWidthBottom = 2;
        style.BorderWidthLeft = 2;
        style.BorderWidthRight = 2;
        style.BorderWidthTop = 2;
        style.BorderColor = new Color(0.7f, 0.6f, 0.3f); // Golden frame
        style.CornerRadiusBottomLeft = 8;
        style.CornerRadiusBottomRight = 8;
        style.CornerRadiusTopLeft = 8;
        style.CornerRadiusTopRight = 8;
        AddThemeStyleboxOverride("panel", style);

        var vbox = new VBoxContainer();
        vbox.AddThemeConstantOverride("separation", 4);
        AddChild(vbox);

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
        _manaLabel.Modulate = new Color(0.9f, 0.8f, 0.4f);
        _manaLabel.Text = "";
        headerHBox.AddChild(_manaLabel);

        // Type Line
        _typeLabel = new Label();
        _typeLabel.AddThemeFontSizeOverride("font_size", 10);
        _typeLabel.Modulate = new Color(0.8f, 0.8f, 0.8f);
        _typeLabel.Text = "Type Line";
        vbox.AddChild(_typeLabel);

        // Oracle Box
        _oracleLabel = new RichTextLabel();
        _oracleLabel.SizeFlagsVertical = SizeFlags.ExpandFill;
        _oracleLabel.BbcodeEnabled = true;
        _oracleLabel.ScrollFollowing = false;
        _oracleLabel.FocusMode = FocusModeEnum.None;
        _oracleLabel.MouseFilter = MouseFilterEnum.Ignore;
        _oracleLabel.AddThemeFontSizeOverride("normal_font_size", 10);
        vbox.AddChild(_oracleLabel);

        // P/T Badge
        _ptLabel = new Label();
        _ptLabel.HorizontalAlignment = HorizontalAlignment.Right;
        _ptLabel.AddThemeFontSizeOverride("font_size", 12);
        _ptLabel.Modulate = new Color(1.0f, 0.9f, 0.5f);
        _ptLabel.Text = "";
        vbox.AddChild(_ptLabel);

        GuiInput += OnGuiInput;
        MouseEntered += OnMouseEntered;
        MouseExited += OnMouseExited;

        if (CardInstance != null)
        {
            UpdateCardVisuals();
        }
    }

    public void Setup(CardInstance cardInstance)
    {
        CardInstance = cardInstance;
        if (IsInsideTree())
        {
            UpdateCardVisuals();
        }
    }

    private void UpdateCardVisuals()
    {
        if (CardInstance?.CardData == null) return;

        var card = CardInstance.CardData;
        _nameLabel.Text = card.FullName;
        _typeLabel.Text = card.FullTypeLine;

        // Mana Cost display
        _manaLabel.Text = ""; // Mana Symbols formatted

        // Oracle text
        _oracleLabel.Clear();
        _oracleLabel.AppendText($"[color=gainsboro]{card.ToString()}[/color]");

        // P/T or loyalty
        _ptLabel.Text = "";
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
    }

    private void OnMouseExited()
    {
        Scale = new Vector2(1.0f, 1.0f);
    }
}
