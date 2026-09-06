using MTG.Core.Cards;
using MTG.Core.Helper;
using MTG.Scryfall.Helper;

namespace MTG.Scryfall._Getter;

public class ScryfallGetCardsNamed
{
    private readonly string apifull;

    public ScryfallGetCardsNamed()
    {
        apifull = $"{ScryfallConnect.apibase}/cards/named";
    }

    //The exact card name to search for, case insenstive.
    public async Task<Result<JsonString>> GetExact(CardRef cref)
    {
        if (cref == null || string.IsNullOrEmpty(cref.Name))
            return Result<JsonString>.Failure("Search string null or empty!");

        string api = $"{apifull}?exact={cref.Name}";
        if (!string.IsNullOrEmpty(cref.Set))
            api += $"&set={cref.Set}";

        return await new Scryfall().GetJson(api);
    }

    //A fuzzy card name to search for.
    public async Task<Result<JsonString>> GetFuzzy(CardRef cref)
    {
        if (cref == null || string.IsNullOrEmpty(cref.Name))
            return Result<JsonString>.Failure("Search string null or empty!");

        string api = $"{apifull}?fuzzy={cref.Name}";
        if (!string.IsNullOrEmpty(cref.Set))
            api += $"&set={cref.Set}";

        return await new Scryfall().GetJson(api);
    }
}