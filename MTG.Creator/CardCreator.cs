using MTG.Core.Cards;
using MTG.Core.Helper;
using MTG.Core.OracleTextParsers;
using MTG.DB;
using MTG.Scryfall._Getter;
using MTG.Scryfall.Helper;

namespace MTG.Creator;

public class CardCreator
{
    private readonly ScryfallCardConverter _converter;

    public CardCreator() : this(new ScryfallCardConverter()) { }

    public CardCreator(ScryfallCardConverter converter)
    {
        _converter = converter;
    }

    public async Task<Result<ICard>> GetByExactName(string name) => await GetByExactName(new CardRef() { Name = name });

    public Task<Result<ICard>> GetByExactName(CardRef cref)
    {
        return GetCard(
            dbQuery: () => AppDbContext.GetByExactName(cref),
            sfQuery: () => new ScryfallGetCardsNamed().GetExact(cref),
            cref: cref
        );
    }

    public Task<Result<ICard>> GetById(CardRef cref)
    {
        return GetCard(
            dbQuery: () => AppDbContext.GetById(cref),
            sfQuery: () => new ScryfallGetCardsId().GetId(cref),
            cref: cref
        );
    }

    private async Task<Result<ICard>> GetCard(
        Func<Result<JsonString>> dbQuery,
        Func<Task<Result<JsonString>>> sfQuery,
        CardRef cref)
    {
        //Get JSON from DB
        var sqlCard = dbQuery();

        if (sqlCard.IsSuccess)
        {
            //Convert into Card
            var card1 = await _converter.DoubleConvert(sqlCard.Value);
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
            var card2 = await _converter.DoubleConvert(json2.Value);
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

    public async Task<Result<ICard>> GetFuzzy(CardRef cred)
    {
        throw new NotImplementedException();
    }
}
