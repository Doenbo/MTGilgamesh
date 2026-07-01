using System.Text.Json.Serialization;

namespace MTG.Scryfall.API.Rulings;

//https://scryfall.com/docs/api/rulings

public class ScryfallRuling
{
    [JsonPropertyName("object")]
    public required string Object { get; init; }

    [JsonPropertyName("oracle_id")]
    public required string Oracle_id { get; init; }

    [JsonPropertyName("source")]
    public required string Source { get; init; }

    [JsonPropertyName("published_at")]
    public required DateTime PublishedAt { get; init; }

    [JsonPropertyName("comment")]
    public required string Comment { get; init; }
}