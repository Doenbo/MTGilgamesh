using MTG.Core.Cards;
using MTG.Core.Helper;
using MTG.Scryfall.Helper;

namespace MTG.Scryfall._Getter;

public class ScryfallGetCardsId
{
    private readonly string apifull;

    public ScryfallGetCardsId()
    {
        apifull = $"{ScryfallConnect.apibase}/cards/";
    }

    //TODO clean up string building in all classes?

    //Returns a single card with the given Scryfall ID.
    public async Task<Result<JsonString>> GetId(CardRef cref)
    {
        if (cref == null || string.IsNullOrEmpty(cref.Id.ToString()))
            return Result<JsonString>.Failure("Search string null or empty!");

        string api = $"{apifull}{cref.Id}";

        return await new Scryfall().GetJson(api);
    }
}
