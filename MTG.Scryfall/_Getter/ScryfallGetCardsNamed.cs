using MTG.Core.Cards;
using MTG.Core.Helper;
using MTG.Scryfall.Helper;

namespace MTG.Scryfall._Getter;

public class ScryfallGetCardsNamed
{
    private readonly string apifull;
    private readonly HttpClient client;

    public ScryfallGetCardsNamed()
    {
        client = ScryfallConnect.GetClient();
        apifull = $"{ScryfallConnect.apibase}/cards/named";
    }


    //The exact card name to search for, case insenstive.
    public Task<Result<JsonString>> GetExact(CardRef cref) => GetJson(cref, "exact");

    //A fuzzy card name to search for.
    public Task<Result<JsonString>> GetFuzzy(CardRef cref) => GetJson(cref, "fuzzy");

    private async Task<Result<JsonString>> GetJson(CardRef cref, string mode)
    {
        if (cref == null || string.IsNullOrEmpty(cref.Name))
            return Result<JsonString>.Failure("Search string null or empty!");

        string api = $"{apifull}?{mode}={cref.Name}";
        if (!string.IsNullOrEmpty(cref.Set))
            api += $"&set={cref.Set}";

        try
        {
            var json = new JsonString(await client.GetStringAsync(api));
            await Task.Delay(500); //So the API doesn't suspend us
            if (json == null)
                return Result<JsonString>.Failure("JSON is null!");

            return Result<JsonString>.Success(json);
        }
        catch (Exception ex)
        {
            return Result<JsonString>.Failure($"Error getting JSON data: {ex}");
        }

    }
}