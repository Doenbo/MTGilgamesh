using System.Text.Json.Serialization;

namespace MTG.Scryfall.API.Errors;

//https://scryfall.com/docs/api/errors
public class ScryfallError
{
    [JsonPropertyName("status")]
    public required int Status { get; set; }

    [JsonPropertyName("code")]
    public required string Code { get; set; }

    [JsonPropertyName("details")]
    public required string details { get; set; }

    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("warnings")]
    public List<string>? Warnings { get; set; }
}
