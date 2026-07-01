using MTG.Scryfall.Helper;

namespace MTG.Scryfall.Tests;

public class ScryfallCardConverterTests
{
    private static readonly string name = "Emmara, Soul of the Accord";
    public static IEnumerable<object[]> ScryfallCardConverterValidTestData()
    {
        yield return new object[]
        {
            new JsonString($"{{" +
                           $"\"object\": \"card\"," +
                           $"\"id\": \"41b930ee-e16b-4612-87de-c03ecc6ff6db\"," +
                           $"\"card_back_id\": \"0aeebaf5-8c7d-4636-9e82-8c27447861f7\"," +
                           $"\"set_id\": \"0aeebaf5-8c7d-4636-9e82-8c27447861f7\"," +
                           $"\"name\": \"{name}\"," +
                           $"\"type_line\": \"Legendary Creature — Elf Cleric\"," +
                           $"\"set_name\": \"Guilds of Ravnica\"," +
                           $"\"set_type\": \"expansion\"," +
                           $"\"set\": \"grn\"," +
                           $"\"lang\": \"en\"," +
                           $"\"collector_number\": \"168\"," +
                           $"\"frame\": \"2015\"," +
                           $"\"rarity\": \"rare\"," +
                           $"\"cmc\": 2," +
                           $"\"story_spotlight\": false," +
                           $"\"textless\": false," +
                           $"\"variation\": false," +
                           $"\"reprint\": false," +
                           $"\"reserved\": false," +
                           $"\"promo\": false," +
                           $"\"digital\": false," +
                           $"\"full_art\": false," +
                           $"\"highres_image\": false," +
                           $"\"booster\": true," +
                           $"\"oversized\": true," +
                           $"\"layout\": \"normal\"," +
                           $"\"image_status\": \"highres_scan\"," +
                           $"\"released_at\": \"2018-10-05\"," +
                           $"\"border_color\": \"black\"," +
                           $"\"color_identity\": [\"G\", \"W\"]," +
                           $"\"keywords\": []," +
                           $"\"finishes\": []," +
                           $"\"games\": []," +
                           $"\"legalities\": {{ \"standard\": \"not_legal\" }}," +
                           $"\"prices\": {{  }}," +
                           $"\"related_uris\": {{  }}," +
                           $"\"uri\": \"https://api.scryfall.com/cards/search\"," +
                           $"\"rulings_uri\": \"https://api.scryfall.com/cards/search\"," +
                           $"\"scryfall_uri\": \"https://api.scryfall.com/cards/search\"," +
                           $"\"scryfall_set_uri\": \"https://api.scryfall.com/cards/search\"," +
                           $"\"prints_search_uri\": \"https://api.scryfall.com/cards/search\"," +
                           $"\"set_search_uri\": \"https://api.scryfall.com/cards/search\"," +
                           $"\"set_uri\": \"https://api.scryfall.com/cards/search\"" +
                           $"}}")};
    }

    [Theory]
    [MemberData(nameof(ScryfallCardConverterValidTestData))]
    public async Task TestGetExactTestSpace(JsonString json)
    {
        var act = ScryfallCardConverter.Convert(json);
        Assert.True(act.IsSuccess);
        Assert.Equal(name, act.Value.Name);
    }
}
