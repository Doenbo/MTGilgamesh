using MTG.Core.Enums;

namespace MTG.Engine.Zones;

public class ExileZone : BaseZone
{
    public override ZoneType Type => ZoneType.Exile;

    public override bool IsPublic => true;
}