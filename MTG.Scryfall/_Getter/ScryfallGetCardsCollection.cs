//using MTG.Core.Cards;
//using MTG.Core.Helper;
//using MTG.Scryfall.API.Cards;
//using MTG.Scryfall.Helper;
//using System.Net.Http.Json;

//namespace MTG.Scryfall._Getter;

//public class ScryfallGetCardsCollection
//{
//    private string apifull;
//    private HttpClient client;

//    public ScryfallGetCardsCollection()
//    {
//        client = ScryfallConnect.GetClient();
//        apifull = $"{ScryfallConnect.apibase}/cards/collection";
//    }

//    public async Task<Result<List<ScryfallCardS>>> GetScryfallCardsCollection(ScryfallCollectionRequest cr)
//    {
//        if (cr == null)
//            return Result<List<ScryfallCardS>>.Failure("Collection Request can't be null!");

//        try
//        {
//            var response = await client.PostAsJsonAsync(apifull, cr);
//            if (!response.IsSuccessStatusCode)
//                return Result<List<ScryfallCardS>>.Failure("API Response failed!");

//            var scryfallResponse = await response.Content.ReadFromJsonAsync<ScryfallCollectionResponse>();
//            if (scryfallResponse == null || scryfallResponse.Data == null)
//                return Result<List<ScryfallCardS>>.Failure("Die Antwort von Scryfall war leer oder fehlerhaft.");

//            return Result<List<ScryfallCardS>>.Success(scryfallResponse.Data);
//        }
//        catch (Exception ex)
//        {
//            return Result<List<ScryfallCardS>>.Failure($"Error while communicating with API: {ex}");
//        }
//    }

//    public async Task<Result<List<Card>>> GetCardsCollection(ScryfallCollectionRequest cr)
//    {
//        if (cr == null)
//            return Result<List<Card>>.Failure("Collection Request can't be null!");

//        try
//        {
//            var response = await client.PostAsJsonAsync(apifull, cr);
//            if (!response.IsSuccessStatusCode)
//                return Result<List<Card>>.Failure("API Response failed!");

//            var scryfallResponse = await response.Content.ReadFromJsonAsync<ScryfallCollectionResponse>();
//            if (scryfallResponse == null || scryfallResponse.Data == null)
//                return Result<List<Card>>.Failure("Die Antwort von Scryfall war leer oder fehlerhaft.");

//            var cards = new List<Card>();
//            foreach (var dto in scryfallResponse.Data)
//            {
//                //var cardResult = ScryfallCardConverter.Convert(dto);

//                //if (cardResult.IsFailure)
//                //    return cardResult.ToFailure<List<Card>>();

//                //cards.Add(cardResult.Value);
//            }
//            return Result<List<Card>>.Success(cards);
//        }
//        catch (Exception ex)
//        {
//            return Result<List<Card>>.Failure($"Error while communicating with API: {ex}");
//        }

//    }
//}

