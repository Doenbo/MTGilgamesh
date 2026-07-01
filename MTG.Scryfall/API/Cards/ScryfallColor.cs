using System.Text.Json.Serialization;

namespace MTG.Scryfall.API.Cards;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ScryfallColor
{
    W,
    U,
    B,
    R,
    G,
    C,
}
