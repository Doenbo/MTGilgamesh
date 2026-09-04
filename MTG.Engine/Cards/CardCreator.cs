using MTG.Core.Cards;
using MTG.Core.Helper;
using MTG.Core.OracleTextParsers;
using MTG.DB;
using MTG.Scryfall._Getter;
using MTG.Scryfall.Helper;

namespace MTG.Engine.Cards;

public static class CardCreator
{
    public static async Task<Result<ICard>> GetExact(CardRef cref)
    {
        //Get JSON from DB
        var sqlCard = AppDbContext.GetExact(cref);

        //TODO which constructor???
        var conv = new ScryfallCardConverter();

        if (sqlCard.IsSuccess)
        {
            //Convert into ScryfallCard Object

            var sfCard1 = conv.Convert(sqlCard.Value);
            if (sfCard1.IsFailure)
                return sfCard1.ToFailure<ICard>();

            //Convert into Card Object
            var card1 = conv.Convert(sfCard1.Value);
            if (card1.IsFailure)
                return card1.ToFailure<ICard>();

            return Result<ICard>.Success(card1.Value);
        }
        else if (sqlCard.IsFailure && sqlCard.Error.StartsWith("Card with the name")) //TODO very bad -> rework
        {
            //Get JSON from Scryfall API
            var api = new ScryfallGetCardsNamed();
            var json2 = await api.GetExact(cref);
            if (json2.IsFailure)
                return json2.ToFailure<ICard>();

            //Convert into ScryfallCard Object
            var sfCard2 = conv.Convert(json2.Value);
            if (sfCard2.IsFailure)
                return sfCard2.ToFailure<ICard>();

            //Convert into Card Object
            var card2 = conv.Convert(sfCard2.Value);
            if (card2.IsFailure)
                return card2.ToFailure<ICard>();

            //Fill CardRef for DB
            cref.Id = card2.Value.Id;
            cref.Set = card2.Value.Set;
            cref.CollectorNumber = card2.Value.CollectorNumber;

            //Write Card in DB
            var result = AppDbContext.InsertCardIntoDb(json2.Value, cref);
            if (result.IsFailure)
                return result.ToFailure<ICard>();

            return Result<ICard>.Success(card2.Value);
        }
        else
        {
            return sqlCard.ToFailure<ICard>();
        }
    }

    public static async Task<Result<ICard>> GetExact(string name) => await GetExact(new CardRef() { Name = name });

    public static async Task<Result<ICard>> GetFuzzy(CardRef cred)
    {
        throw new NotImplementedException();
    }
}
