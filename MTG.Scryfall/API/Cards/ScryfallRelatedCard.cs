using System.Text.Json.Serialization;

namespace MTG.Scryfall.API.Cards;

public class ScryfallRelatedCard
{
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    [JsonPropertyName("object")]
    public required string Object { get; init; }

    [JsonPropertyName("component")]
    public required string Component { get; init; }

    [JsonPropertyName("name")]
    public required string Name { get; init; }

    [JsonPropertyName("type_line")]
    public required string TypeLine { get; init; }

    [JsonPropertyName("uri")]
    public required Uri Uri { get; init; }
}
