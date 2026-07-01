using System.Text.Json.Serialization;

namespace MTG.Scryfall.API.Cards;

public class ScryfallCardFace
{
    [JsonPropertyName("artist")]
    public string? Artist { get; init; }

    [JsonPropertyName("artist_id")]
    public string? ArtistId { get; init; }

    [JsonPropertyName("cmc")]
    public float? CMC { get; init; }

    [JsonPropertyName("color_indicator")]
    public List<string>? ColorIndicator { get; init; }

    [JsonPropertyName("colors")]
    public List<string>? Colors { get; init; }

    [JsonPropertyName("defense")]
    public string? Defense { get; init; }

    [JsonPropertyName("flavor_text")]
    public string? FlavorText { get; init; }

    [JsonPropertyName("illustration_id")]
    public string? IllustrationId { get; init; }

    [JsonPropertyName("image_uris")]
    public Dictionary<string, string>? ImageUris { get; init; }

    [JsonPropertyName("layout")]
    public string? Layout { get; init; }

    [JsonPropertyName("loyalty")]
    public string? Loyalty { get; init; }

    [JsonPropertyName("mana_cost")]
    public required string ManaCost { get; init; }

    [JsonPropertyName("name")]
    public required string Name { get; init; }

    [JsonPropertyName("object")]
    public required string Object { get; init; }

    [JsonPropertyName("oracle_id")]
    public string? OracleId { get; init; }

    [JsonPropertyName("oracle_text")]
    public string? OracleText { get; init; }

    [JsonPropertyName("power")]
    public string? Power { get; init; }

    [JsonPropertyName("printed_name")]
    public string? PrintedName { get; init; }

    [JsonPropertyName("printed_text")]
    public string? PrintedText { get; init; }

    [JsonPropertyName("printed_type_line")]
    public string? PrintedTypeLine { get; init; }

    [JsonPropertyName("toughness")]
    public string? Toughness { get; init; }

    [JsonPropertyName("type_line")]
    public required string TypeLine { get; init; }

    [JsonPropertyName("watermark")]
    public string? Watermark { get; init; }
}
