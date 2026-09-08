using MTG.Core.Helper;
using MTG.Scryfall.Helper;

namespace MTG.Scryfall;

public class Scryfall
{
    private readonly HttpClient client;

    public Scryfall()
    {
        client = ScryfallConnect.GetClient();
    }

    public async Task<Result<JsonString>> GetJson(string api)
    {
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
