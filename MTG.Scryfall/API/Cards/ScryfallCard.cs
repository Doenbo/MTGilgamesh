using System.Text.Json.Serialization;

namespace MTG.Scryfall.API.Cards;

public class ScryfallCard
{
    //Core Fields
    [JsonPropertyName("arena_id")]
    public int? ArenaId { get; init; }

    [JsonPropertyName("id")]
    public required string Id { get; init; } //our primary key

    [JsonPropertyName("lang")]
    public required string Lang { get; init; }

    [JsonPropertyName("mtgo_id")]
    public int? MtgoId { get; init; }

    [JsonPropertyName("mtgo_foil_id")]
    public int? MtgoFoilId { get; init; }

    [JsonPropertyName("multiverse_ids")]
    public List<int>? MultiverseIds { get; init; }

    [JsonPropertyName("resource_id")]
    public string? ResourceId { get; init; }

    [JsonPropertyName("tcgplayer_id")]
    public int? TcgplayerId { get; init; }

    [JsonPropertyName("tcgplayer_etched_id")]
    public int? TcgplayerEtchedId { get; init; }

    [JsonPropertyName("cardmarket_id")]
    public int? CardmarketId { get; init; }

    [JsonPropertyName("object")]
    public required string Object { get; init; }

    [JsonPropertyName("layout")]
    public required string Layout { get; init; }

    [JsonPropertyName("oracle_id")]
    public string? OracleId { get; init; }

    [JsonPropertyName("prints_search_uri")]
    public required Uri PrintsSearchUri { get; init; }

    [JsonPropertyName("rulings_uri")]
    public required Uri RulingsUri { get; init; }

    [JsonPropertyName("scryfall_uri")]
    public required Uri ScryfallUri { get; init; }

    [JsonPropertyName("uri")]
    public required Uri Uri { get; init; }

    //Gameplay Fields

    [JsonPropertyName("all_parts")]
    public List<ScryfallRelatedCard>? AllParts { get; init; } //Related Card Object

    [JsonPropertyName("card_faces")]
    public List<ScryfallCardFace>? CardFaces { get; init; } //Card Face Object

    [JsonPropertyName("cmc")]
    public required float CMC { get; init; }

    [JsonPropertyName("color_identity")]
    public required List<string> ColorIdentity { get; init; } //enum

    [JsonPropertyName("color_indicator")]
    public List<string>? ColorIndicator { get; init; } //enum

    [JsonPropertyName("colors")]
    public List<string>? Colors { get; init; } //enum

    [JsonPropertyName("defense")]
    public string? Defense { get; init; }

    [JsonPropertyName("edhrec_rank")]
    public int? EdhrecRank { get; init; }

    [JsonPropertyName("game_changer")]
    public bool? GameChanger { get; init; }

    [JsonPropertyName("hand_modifier")]
    public string? HandModifier { get; init; }

    [JsonPropertyName("keywords")]
    public required List<string> Keywords { get; init; } //enum

    [JsonPropertyName("legalities")]
    public required Dictionary<string, string> Legalities { get; init; }

    [JsonPropertyName("life_modifier")]
    public string? LifeModifier { get; init; }

    [JsonPropertyName("loyalty")]
    public string? Loyalty { get; init; }

    [JsonPropertyName("mana_cost")]
    public string? ManaCost { get; init; }

    [JsonPropertyName("name")]
    public required string Name { get; init; }

    [JsonPropertyName("oracle_text")]
    public string? OracleText { get; init; }

    [JsonPropertyName("penny_rank")]
    public int? PennyRank { get; init; }

    [JsonPropertyName("power")]
    public string? Power { get; init; }

    [JsonPropertyName("produced_mana")]
    public List<ScryfallColor>? ProducedMana { get; init; }

    [JsonPropertyName("reserved")]
    public required bool Reserved { get; init; }

    [JsonPropertyName("toughness")]
    public string? Toughness { get; init; }

    [JsonPropertyName("type_line")]
    public required string TypeLine { get; init; }

    //Print Fields

    [JsonPropertyName("artist")]
    public string? Artist { get; init; }

    [JsonPropertyName("artist_ids")]
    public List<string>? ArtistIds { get; init; }

    [JsonPropertyName("attraction_lights")]
    public List<string>? AttractionLights { get; init; }

    [JsonPropertyName("booster")]
    public required bool Booster { get; init; }

    [JsonPropertyName("border_color")]
    public required string BorderColor { get; init; }

    [JsonPropertyName("card_back_id")]
    public required string CardBackId { get; init; }

    [JsonPropertyName("collector_number")]
    public required string CollectorNumber { get; init; }

    [JsonPropertyName("content_warning")]
    public bool? ContentWarning { get; init; }

    [JsonPropertyName("digital")]
    public required bool Digital { get; init; }

    [JsonPropertyName("finishes")]
    public required List<string> Finishes { get; init; } //enum

    [JsonPropertyName("flavor_name")]
    public string? FlavorName { get; init; }

    [JsonPropertyName("flavor_text")]
    public string? FlavorText { get; init; }

    [JsonPropertyName("frame_effects")]
    public List<string>? FrameEffects { get; init; }

    [JsonPropertyName("frame")]
    public required string Frame { get; init; }

    [JsonPropertyName("full_art")]
    public required bool FullArt { get; init; }

    [JsonPropertyName("games")]
    public required List<string> Games { get; init; }

    [JsonPropertyName("highres_image")]
    public required bool HighresImage { get; init; }

    [JsonPropertyName("illustration_id")]
    public string? IllustrationId { get; init; }

    [JsonPropertyName("image_status")]
    public required string ImageStatus { get; init; }

    [JsonPropertyName("image_uris")]
    public Dictionary<string, string>? ImageUris { get; init; }

    [JsonPropertyName("oversized")]
    public required bool Oversized { get; init; }

    [JsonPropertyName("prices")]
    public required Dictionary<string, string> Prices { get; init; }

    [JsonPropertyName("printed_name")]
    public string? PrintedName { get; init; }

    [JsonPropertyName("printed_text")]
    public string? PrintedText { get; init; }

    [JsonPropertyName("printed_type_line")]
    public string? PrintedTypeLine { get; init; }

    [JsonPropertyName("promo")]
    public required bool Promo { get; init; }

    [JsonPropertyName("promo_types")]
    public List<string>? PromoTypes { get; init; }

    [JsonPropertyName("purchase_uris")]
    public Dictionary<string, string>? PurchaseUris { get; init; }

    [JsonPropertyName("rarity")]
    public required string Rarity { get; init; } //enum

    [JsonPropertyName("related_uris")]
    public required Dictionary<string, string> RelatedUris { get; init; }

    [JsonPropertyName("released_at")]
    public required DateTime ReleasedAt { get; init; }

    [JsonPropertyName("reprint")]
    public required bool Reprint { get; init; }

    [JsonPropertyName("scryfall_set_uri")]
    public required Uri ScryfallSetUri { get; init; }

    [JsonPropertyName("set_name")]
    public required string SetName { get; init; }

    [JsonPropertyName("set_search_uri")]
    public required Uri SetSearchUri { get; init; }

    [JsonPropertyName("set_type")]
    public required string SetType { get; init; }

    [JsonPropertyName("set_uri")]
    public required Uri SetUri { get; init; }

    [JsonPropertyName("set")]
    public required string Set { get; init; }

    [JsonPropertyName("set_id")]
    public required string SetId { get; init; }

    [JsonPropertyName("story_spotlight")]
    public required bool StorySpotlight { get; init; }

    [JsonPropertyName("textless")]
    public required bool Textless { get; init; }

    [JsonPropertyName("variation")]
    public required bool Variation { get; init; }

    [JsonPropertyName("variation_of")]
    public string? VariationOf { get; init; }

    [JsonPropertyName("security_stamp")]
    public string? SecurityStamp { get; init; }

    [JsonPropertyName("watermark")]
    public string? Watermark { get; init; }

    [JsonPropertyName("preview")]
    public Dictionary<string, string>? Preview { get; init; }
}
