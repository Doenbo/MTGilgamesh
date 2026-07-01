namespace MTG.Scryfall.Helper;

public static class ScryfallConnect
{
    public static readonly string apibase = "https://api.scryfall.com";

    private static readonly string appName = "MTG-Sandbox";
    private static readonly string appVersion = "0.1";
    private static readonly string mail = "mtg.sandbox@gmail.com";

    public static HttpClient GetClient()
    {
        var client = new HttpClient();
        client.DefaultRequestHeaders.Add("User-Agent", $"{appName}/{appVersion} ({mail})");
        client.DefaultRequestHeaders.Add("Accept", "application/json");
        return client;
    }
}
