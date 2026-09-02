using MTG.Core.Abilities;
using MTG.Core.Cards;
using MTG.Core.Components;
using MTG.Core.Enums;
using MTG.Core.Helper;
using MTG.Core.OracleTextParsers;
using MTG.Core.Parser;
using MTG.Core.Properties;
using MTG.Scryfall.API.Cards;
using System.Text.Json;

namespace MTG.Scryfall.Helper;

public class ScryfallCardConverter : IScryfallCardConverter
{
    private readonly IOracleTextParser _oracleTextParser;

    public ScryfallCardConverter() : this(new OracleTextParser()) { }

    public ScryfallCardConverter(IOracleTextParser oracleTextParser)
    {
        _oracleTextParser = oracleTextParser;
    }

    public Result<ICard> DoubleConvert(JsonString json)
    {

        if (json == null || string.IsNullOrEmpty(json.Value))
            return Result<ICard>.Failure("JSon can't be null or empty!");

        var sfCard = Convert(json);
        if (sfCard.IsFailure)
            return sfCard.ToFailure<ICard>();

        var card = Convert(sfCard.Value);
        return card.IsSuccess ? card : card.ToFailure<ICard>();
    }

    public Result<ScryfallCard> Convert(JsonString json)
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

    private Result<ICardFace> CreateCardFace(
        ICard card, string name, string typeline, string? oracleText, string? manaCost, float cmc,
        List<string> colorIdentity, List<string>? colorIndicator, List<string>? colors, string? power, string? toughness,
        string? defense, string? loyalty, List<string> keywords)
    {

        //TypeLine
        var typelineres = TypeLine.Create(typeline);
        if (typelineres.IsFailure)
            return typelineres.ToFailure<ICardFace>();

        //Name + Face
        var cardfaceres = CardFaceFactory.Create(name, typelineres.Value, oracleText);
        if (cardfaceres.IsFailure)
            return cardfaceres.ToFailure<ICardFace>();
        var cardface = cardfaceres.Value;

        //ManaCost
        var manaCostComponent = ManaCostComponent.Create(manaCost);
        if (manaCostComponent.IsFailure)
            return manaCostComponent.ToFailure<ICardFace>();

        cardface.AddComponent(manaCostComponent.Value);

        //CMC
        var cmcres = manaCostComponent.Value.GetCMC();
        if (cmcres.IsFailure)
            return cmcres.ToFailure<ICardFace>();
        if (cmc != -1 && cmc.IsNotEqualTo(cmcres.Value))
            return Result<ICardFace>.Failure($"CMCs do not match for the card {name}!");

        //Color
        var colorComponent = ColorComponent.Create(colors, colorIdentity, colorIndicator);
        if (colorComponent.IsFailure)
            return colorComponent.ToFailure<ICardFace>();
        cardface.AddComponent(colorComponent.Value);

        //Components

        //Creature
        if (cardface.IsCreature())
        {
            var creature = CreatureComponent.Create(power, toughness);
            if (creature.IsFailure)
                return creature.ToFailure<ICardFace>();

            cardface.AddComponent(creature.Value);
        }

        //Battle
        if (cardface.IsBattle())
        {
            var battle = BattleComponent.Create(defense);
            if (battle.IsFailure)
                return battle.ToFailure<ICardFace>();

            cardface.AddComponent(battle.Value);
        }

        //Planeswalker
        if (cardface.IsPlaneswalker())
        {
            var planeswalker = PlaneswalkerComponent.Create(loyalty);
            if (planeswalker.IsFailure)
                return planeswalker.ToFailure<ICardFace>();

            cardface.AddComponent(planeswalker.Value);
        }

        // Oracle Text Parser
        if (!string.IsNullOrWhiteSpace(oracleText))
        {
            var parseResult = _oracleTextParser.Parse(oracleText, new CardContext(name));

            if (parseResult.IsFailure)
                return parseResult.ToFailure<ICardFace>();

            cardface.AddComponents(parseResult.Value);
        }

        return Result<ICardFace>.Success(cardface);
    }

    public Result<ICard> Convert(ScryfallCard dto)
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
            if (dto.CardFaces == null) //Single Faced
            {
                var noFace = CreateCardFace(card, dto.Name, dto.TypeLine, dto.OracleText, dto.ManaCost,
                                            dto.CMC, dto.ColorIdentity, dto.ColorIndicator, dto.Colors,
                                            dto.Power, dto.Toughness, dto.Defense, dto.Loyalty, dto.Keywords);
                if (noFace.IsFailure)
                    return noFace.ToFailure<ICard>();

                card.Faces.Add(noFace.Value);
            }
            else //Multi Faces
            {
                var cardFace = dto.CardFaces[i];
                if (cardFace == null || cardFace.Object != "card_face")
                    return Result<ICard>.Failure("Object is not a card face!");

                var iFace = CreateCardFace(card, cardFace.Name, cardFace.TypeLine, cardFace.OracleText, cardFace.ManaCost,
                                           cardFace.CMC ?? -1, null!, cardFace.ColorIndicator, cardFace.Colors,
                                           cardFace.Power, cardFace.Toughness, cardFace.Defense, cardFace.Loyalty,
                                           //TODO NO KEYWORDS??
                                           dto.Keywords);
                if (iFace.IsFailure)
                    return iFace.ToFailure<ICard>();

                card.Faces.Add(iFace.Value);
            }
        }

        //Card Properties (not on Face)
        card.Id = new Guid(dto.Id);
        card.Lang = dto.Lang;
        card.SetName = dto.SetName;

        //Legalities
        foreach (var sLegality in dto.Legalities.ToList())
        {
            if (!Enum.TryParse(Conversions.ToCamelCase(sLegality.Key), out Format eFormat))
                return Result<ICard>.Failure($"Could not parse {sLegality.Key} to Format enum!");

            if (!Enum.TryParse(Conversions.ToCamelCase(sLegality.Value), out Legality eLegality))
                return Result<ICard>.Failure($"Could not parse {sLegality.Value} to Legality enum!");

            card.Legalities.Add(eFormat, eLegality);
        }

        //Image Uris
        if (dto.ImageUris != null)
        {
            foreach (var sImageUri in dto.ImageUris.ToList())
            {
                if (!Enum.TryParse(Conversions.ToCamelCase(sImageUri.Key), out ImageSize eImageUri))
                    return Result<ICard>.Failure($"Could not parse {sImageUri.Key} to enum!");
                card.ImageUris.Add(eImageUri, new Uri(sImageUri.Value));
            }
        }

        return Result<ICard>.Success(card);
    }
}
