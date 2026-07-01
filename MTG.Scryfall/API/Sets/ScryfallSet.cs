using System.Text.Json.Serialization;

namespace MTG.Scryfall.API.Sets;

//https://scryfall.com/docs/api/sets

public class ScryfallSet
{
    [JsonPropertyName("object")]
    public required string Object { get; init; }

    [JsonPropertyName("id")]
    public required string Id { get; init; }

    [JsonPropertyName("code")]
    public required string Code { get; init; }

    [JsonPropertyName("mtgo_code")]
    public string? MtgoCode { get; init; }

    [JsonPropertyName("arena_code")]
    public string? ArenaCode { get; init; }

    [JsonPropertyName("tcgplayer_id")]
    public int? TcgplayerId { get; init; }

    [JsonPropertyName("name")]
    public required string Name { get; init; }

    [JsonPropertyName("set_type")]
    public required string SetType { get; init; }

    [JsonPropertyName("released_at")]
    public DateTime? released_at { get; init; }

    [JsonPropertyName("block_code")]
    public string? BlockCode { get; init; }

    [JsonPropertyName("block")]
    public string? Block { get; init; }

    [JsonPropertyName("parent_set_code")]
    public string? ParentSetCode { get; init; }

    [JsonPropertyName("card_count")]
    public required int CardCount { get; init; }

    [JsonPropertyName("printed_size")]
    public int? PrintedSize { get; init; }

    [JsonPropertyName("digital")]
    public required bool Digital { get; init; }

    [JsonPropertyName("foil_only")]
    public required bool FoilOnly { get; init; }

    [JsonPropertyName("nonfoil_only")]
    public required bool NonfoilOnly { get; init; }

    [JsonPropertyName("scryfall_uri")]
    public required Uri ScryfallUri { get; init; }

    [JsonPropertyName("uri")]
    public required Uri Uri { get; init; }

    [JsonPropertyName("icon_svg_uri")]
    public required Uri IconSvgUri { get; init; }

    [JsonPropertyName("search_uri")]
    public required Uri SearchUri { get; init; }
}