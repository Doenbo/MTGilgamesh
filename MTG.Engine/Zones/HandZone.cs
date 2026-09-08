using MTG.Core.Enums;

namespace MTG.Engine.Zones;

public class HandZone : BaseZone
{
    public override ZoneType Type => ZoneType.Hand;

    public override bool IsPublic => false;

    public int MaximumHandSize { get; set; } = 7;
}