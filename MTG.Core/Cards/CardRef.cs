using System.Text.Json.Serialization;

namespace MTG.Core.Cards;

public class CardRef
{
    [JsonIgnore]
    public int Quantity { get; set; }

    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("set")]
    public string Set { get; set; }

    [JsonPropertyName("collector_number")]
    public string CollectorNumber { get; set; }

    [JsonIgnore]
    public string Type { get; set; }
}