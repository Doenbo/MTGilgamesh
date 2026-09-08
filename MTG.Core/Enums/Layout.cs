using System.Text.Json.Serialization;

namespace MTG.Core.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Layout
{
    Normal,
    Split,
    Flip,
    Transform,
    ModalDfc,
    Meld,
    Leveler,
    Class,
    Case,
    Sage,
    Adventure,
    Prepare,
    Mutate,
    Prototype,
    Battle,
    Planar,
    Scheme,
    Vanguard,
    Token,
    DoubleFacedToken,
    Emblem,
    Augment,
    Host,
    ArtSeries,
    ReversibleCard,
    FrontCard,
}
