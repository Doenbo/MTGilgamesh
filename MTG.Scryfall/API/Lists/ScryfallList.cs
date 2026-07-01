using System.Text.Json.Serialization;

namespace MTG.Scryfall.API.Lists;

//https://scryfall.com/docs/api/lists
public class ScryfallList
{
    [JsonPropertyName("object")]
    public required string Object { get; init; }

    [JsonPropertyName("data")]
    public required List<string> Data { get; init; }

    [JsonPropertyName("has_more")]
    public required bool HasMore { get; init; }

    [JsonPropertyName("next_page")]
    public Uri? NextPage { get; init; }

    [JsonPropertyName("total_cards")]
    public int? total_cards { get; init; }

    [JsonPropertyName("warnings")]
    public List<string>? Warnings { get; init; }
}
