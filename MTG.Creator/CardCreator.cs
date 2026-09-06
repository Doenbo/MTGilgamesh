using MTG.Core.Cards;
using MTG.Core.Helper;
using MTG.Core.OracleTextParsers;
using MTG.DB;
using MTG.Scryfall._Getter;
using MTG.Scryfall.Helper;

namespace MTG.Creator;

public static class CardCreator
{
    public static async Task<Result<ICard>> GetByExactName(string name) => await GetByExactName(new CardRef() { Name = name });

    public static Task<Result<ICard>> GetByExactName(CardRef cref)
    {
        return GetCard(
            dbQuery: () => AppDbContext.GetByExactName(cref),
            sfQuery: () => new ScryfallGetCardsNamed().GetExact(cref),
            cref: cref
        );
    }

    public static Task<Result<ICard>> GetById(CardRef cref)
    {
        return GetCard(
            dbQuery: () => AppDbContext.GetById(cref),
            sfQuery: () => new ScryfallGetCardsId().GetId(cref),
            cref: cref
        );
    }

    private static async Task<Result<ICard>> GetCard(
        Func<Result<JsonString>> dbQuery,
        Func<Task<Result<JsonString>>> sfQuery,
        CardRef cref)
    {
        //Get JSON from DB
        var sqlCard = dbQuery();
        var conv = new ScryfallCardConverter();

        if (sqlCard.IsSuccess)
        {
            //Convert into Card
            var card1 = await conv.DoubleConvert(sqlCard.Value);
            if (card1.IsFailure)
                return card1.ToFailure<ICard>();

            return Result<ICard>.Success(card1.Value);
        }

        if (sqlCard.IsFailure && sqlCard.Error.StartsWith("Card with the name")) //TODO very bad -> rework
        {
            //Get JSON from Scryfall API
            var json2 = await sfQuery();
            if (json2.IsFailure)
                return json2.ToFailure<ICard>();

            //Convert into Card
            var card2 = await conv.DoubleConvert(json2.Value);
            if (card2.IsFailure)
                return card2.ToFailure<ICard>();

            //Fill CardRef for DB
            cref.Name = card2.Value.Name;
            cref.Id = card2.Value.Id;
            cref.Set = card2.Value.Set;
            cref.CollectorNumber = card2.Value.CollectorNumber;

            //Write Card in DB
            var result = AppDbContext.InsertCardIntoDb(json2.Value, cref);
            if (result.IsFailure)
                return result.ToFailure<ICard>();

            return Result<ICard>.Success(card2.Value);
        }

        return sqlCard.ToFailure<ICard>();
    }

    public static async Task<Result<ICard>> GetFuzzy(CardRef cred)
    {
        throw new NotImplementedException();
    }
}
