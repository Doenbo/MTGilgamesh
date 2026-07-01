using System.Text.Json.Serialization;

namespace MTG.Scryfall.API.Catalogs;

//https://scryfall.com/docs/api/catalogs

public class ScryfallCatalog
{
    [JsonPropertyName("object")]
    public required string Object { get; set; }

    [JsonPropertyName("uri")]
    public required Uri Uri { get; set; }

    [JsonPropertyName("total_values")]
    public required int TotalValues { get; set; }

    [JsonPropertyName("data")]
    public required List<string> Data { get; set; }
}