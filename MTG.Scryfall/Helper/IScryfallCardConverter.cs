using MTG.Core.Cards;
using MTG.Core.Helper;
using MTG.Scryfall.API.Cards;

namespace MTG.Scryfall.Helper;

public interface IScryfallCardConverter
{
    public Result<ICard> DoubleConvert(JsonString json);
    public Result<ScryfallCard> Convert(JsonString json);
}