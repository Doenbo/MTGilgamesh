using MTG.Core.Cards;
using MTG.Core.Helper;
using MTG.Scryfall.API.Cards;

namespace MTG.Scryfall.Helper;

public interface IScryfallCardConverter
{
    public Task<Result<ICard>> DoubleConvert(JsonString json);
    public Task<Result<ScryfallCard>> Convert(JsonString json);
}