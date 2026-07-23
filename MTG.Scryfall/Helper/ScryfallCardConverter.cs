using MTG.Core.Cards;
using MTG.Core.Components;
using MTG.Core.Enums;
using MTG.Core.Helper;
using MTG.Core.Properties;
using MTG.Scryfall.API.Cards;
using System.Text.Json;

namespace MTG.Scryfall.Helper;

public class ScryfallCardConverter
{
    public static Result<ICard> DoubleConvert(JsonString json)
    {

        if (json == null || string.IsNullOrEmpty(json.Value))
            return Result<ICard>.Failure("JSon can't be null or empty!");

        var sfCard = Convert(json);
        if (sfCard.IsFailure)
            return sfCard.ToFailure<ICard>();

        var card = Convert(sfCard.Value);
        return card.IsSuccess ? card : card.ToFailure<ICard>();
    }

    public static Result<ScryfallCard> Convert(JsonString json)
    {
        if (json == null || string.IsNullOrEmpty(json.Value))
            return Result<ScryfallCard>.Failure("JSon can't be null or empty!");

        ScryfallCard? sfCard;
        try
        {
            sfCard = JsonSerializer.Deserialize<ScryfallCard>(json.Value);
        }
        catch (Exception ex)
        {
            return Result<ScryfallCard>.Failure($"Cannot deserialize string: {ex}");
        }
        return sfCard == null ? Result<ScryfallCard>.Failure($"Deserializing result is null!") :
                                Result<ScryfallCard>.Success(sfCard);
    }

    private static Result<ICardFace> CreateCardFace(string name, string typeline, string oracleText, string manaCost,
                                                   float cmc, List<string> colorIdentity, List<string> colorIndicator,
                                                   List<string> colors, string power, string toughness, string defense,
                                                   string loyalty, List<string> keywords)
    {

        //TypeLine
        var typelineres = TypeLine.Create(typeline);
        if (typelineres.IsFailure)
            return typelineres.ToFailure<ICardFace>();

        //Name + Face
        var cardfaceres = CardFaceFactory.Create(name, typelineres.Value);
        if (cardfaceres.IsFailure)
            return cardfaceres.ToFailure<ICardFace>();
        var cardface = cardfaceres.Value;

        //Face Properties
        cardface.OracleText = oracleText;

        //ManaCost
        var mana = ManaComponent.Create(manaCost);
        if (mana.IsFailure)
            return mana.ToFailure<ICardFace>();

        cardface.AddComponent(mana.Value);

        //CMC
        if (cmc != -1 && cmc.IsNotEqualTo(mana.Value.CMC))
            return Result<ICardFace>.Failure($"CMCs do not match for the card {name}!");

        //Color
        var colorComponent = ColorComponent.Create(colorIdentity, colorIndicator, colors);
        if (colorComponent.IsFailure)
            return colorComponent.ToFailure<ICardFace>();
        cardface.AddComponent(colorComponent.Value);

        //TODO auslagern?
        if (cardface.IsCreature())
        {
            //TODO NULL IN CREATE ODER DAVOR???
            if (power == null || toughness == null)
                return Result<ICardFace>.Failure($"Power and Toughness can't be null!");

            var creature = CreatureComponent.Create(power, toughness);
            if (creature.IsFailure)
                return creature.ToFailure<ICardFace>();

            cardface.AddComponent(creature.Value);
        }

        if (cardface.IsBattle())
        {
            if (defense == null)
                return Result<ICardFace>.Failure($"Defense can't be null!");

            var battle = BattleComponent.Create(defense);
            if (battle.IsFailure)
                return battle.ToFailure<ICardFace>();

            cardface.AddComponent(battle.Value);
        }

        if (cardface.IsPlaneswalker())
        {
            if (loyalty == null)
                return Result<ICardFace>.Failure($"Loyalty can't be null!");

            var planeswalker = PlaneswalkerComponent.Create(loyalty);
            if (planeswalker.IsFailure)
                return planeswalker.ToFailure<ICardFace>();

            cardface.AddComponent(planeswalker.Value);
        }

        foreach (var keyword in keywords)
        {
            if (Enum.TryParse<KeywordAbility>(Conversions.ToCamelCase(keyword), true, out var ability))
                cardface.KeywordAbilities.Add(ability);

            else if (Enum.TryParse<KeywordAction>(Conversions.ToCamelCase(keyword), true, out var action))
                cardface.KeywordActions.Add(action);

            else if (Enum.TryParse<AbilityWord>(Conversions.ToCamelCase(keyword), true, out var word))
                cardface.AbilityWords.Add(word);

            else
                return Result<ICardFace>.Failure($"Could not parse {keyword} to Keyword enum!");
        }

        return Result<ICardFace>.Success(cardface);
    }

    public static Result<ICard> Convert(ScryfallCard dto)
    {
        if (dto.Object != "card")
            return Result<ICard>.Failure("Object is not a card!");

        //Create Card
        var cardres = CardFactory.Create(dto.Name, dto.Set, dto.CollectorNumber, dto.TypeLine);
        if (cardres.IsFailure)
            return cardres.ToFailure<ICard>();
        var card = cardres.Value;

        //Create Faces
        var amountFaces = dto.CardFaces == null ? 1 : dto.CardFaces.Count;
        for (var i = 0; i < amountFaces; i++)
        {
            if (dto.CardFaces == null)
            {
                var noFace = CreateCardFace(dto.Name, dto.TypeLine, dto.OracleText, dto.ManaCost,
                                            dto.CMC, dto.ColorIdentity, dto.ColorIndicator, dto.Colors,
                                            dto.Power, dto.Toughness, dto.Defense, dto.Layout, dto.Keywords);
                if (noFace.IsFailure)
                    return noFace.ToFailure<ICard>();

                card.Faces.Add(noFace.Value);
            }
            else
            {
                var cardFace = dto.CardFaces[i];
                if (cardFace == null || cardFace.Object != "card_face")
                    return Result<ICard>.Failure("Object is not a card face!");

                var iFace = CreateCardFace(cardFace.Name, cardFace.TypeLine, cardFace.OracleText, cardFace.ManaCost,
                                           cardFace.CMC ?? -1, null, cardFace.ColorIndicator, cardFace.Colors,
                                           cardFace.Power, cardFace.Toughness, cardFace.Defense, cardFace.Layout,
                                           //TODO NO KEYWORDS??
                                           dto.Keywords);
                if (iFace.IsFailure)
                    return iFace.ToFailure<ICard>();

                card.Faces.Add(iFace.Value);
            }
        }

        //Card Properties
        card.Id = new Guid(dto.Id);
        card.Lang = dto.Lang;
        card.SetName = dto.SetName;

        foreach (var sLegality in dto.Legalities)
        {
            if (!Enum.TryParse(Conversions.ToCamelCase(sLegality.Key), out Format eFormat))
                return Result<ICard>.Failure($"Could not parse {sLegality.Key} to Format enum!");

            if (!Enum.TryParse(Conversions.ToCamelCase(sLegality.Value), out Legality eLegality))
                return Result<ICard>.Failure($"Could not parse {sLegality.Value} to Legality enum!");

            card.Legalities.Add(eFormat, eLegality);
        }

        if (dto.ImageUris != null)
        {
            foreach (var sImageUri in dto.ImageUris)
            {
                if (!Enum.TryParse(Conversions.ToCamelCase(sImageUri.Key), out ImageSize eImageUri))
                    return Result<ICard>.Failure($"Could not parse {sImageUri.Key} to enum!");
                card.ImageUris.Add(eImageUri, new Uri(sImageUri.Value));
            }
        }

        return Result<ICard>.Success(card);
    }
}
