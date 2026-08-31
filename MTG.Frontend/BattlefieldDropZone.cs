using Godot;
using System;

namespace MTG.Frontend;

public partial class BattlefieldDropZone : PanelContainer
{
    public Action<CardNode> OnCardDropped { get; set; }

    public override bool _CanDropData(Vector2 atPosition, Variant data)
    {
        var obj = data.AsGodotObject();
        return obj is CardNode;
    }

    public override void _DropData(Vector2 atPosition, Variant data)
    {
        var obj = data.AsGodotObject();
        if (obj is CardNode cardNode)
        {
            OnCardDropped?.Invoke(cardNode);
        }
    }
}
