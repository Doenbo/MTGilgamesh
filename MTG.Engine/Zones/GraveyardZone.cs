using MTG.Core.Enums;

namespace MTG.Engine.Zones;

public class GraveyardZone : BaseZone
{
    public override ZoneType Type => ZoneType.Graveyard;

    public override bool IsPublic => true;
}