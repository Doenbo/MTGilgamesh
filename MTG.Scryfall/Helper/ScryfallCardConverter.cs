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

public class ScryfallCardConverter(IOracleTextParser oracleTextParser, IManaSymbolParser manaParser)
    : IScryfallCardConverter
{
    public ScryfallCardConverter() : this(new OracleTextParser(), new ManaSymbolParser()) { }

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
        string name, string typeline, string? oracleText, string? manaCost, float cmc, List<string>? colors,
        List<string>? colorIndicator, string? power, string? toughness, string? defense, string? loyalty)
    {
        var components = new List<ICardComponent>();

        //TypeLine
        var typelineres = TypeLine.Create(typeline);
        if (typelineres.IsFailure)
            return typelineres.ToFailure<ICardFace>();

        //ManaCost
        var manaCostComponent = ManaCostComponent.Create(manaCost);
        if (manaCostComponent.IsFailure)
            return manaCostComponent.ToFailure<ICardFace>();

        components.AddRange(manaCostComponent.Value);

        //CMC
        var cmcres = manaCostComponent.Value.GetCMC();
        if (cmcres.IsFailure)
            return cmcres.ToFailure<ICardFace>();
        if (cmc != -1 && cmc.IsNotEqualTo(cmcres.Value))
            return Result<ICardFace>.Failure($"CMCs do not match for the card {name}!");

        //Color
        var colorComponent = ColorComponent.Create(manaParser, colors, colorIndicator);
        if (colorComponent.IsFailure)
            return colorComponent.ToFailure<ICardFace>();
        components.AddRange(colorComponent.Value);

        //Components

        //Creature
        if (typelineres.Value.IsCreature())
        {
            var creature = CreatureComponent.Create(power, toughness);
            if (creature.IsFailure)
                return creature.ToFailure<ICardFace>();

            components.AddRange(creature.Value);
        }

        //Battle
        if (typelineres.Value.IsBattle())
        {
            var battle = BattleComponent.Create(defense);
            if (battle.IsFailure)
                return battle.ToFailure<ICardFace>();

            components.AddRange(battle.Value);
        }

        //Planeswalker
        if (typelineres.Value.IsPlaneswalker())
        {
            var planeswalker = PlaneswalkerComponent.Create(loyalty);
            if (planeswalker.IsFailure)
                return planeswalker.ToFailure<ICardFace>();

            components.AddRange(planeswalker.Value);
        }

        // Oracle Text Parser
        if (!string.IsNullOrWhiteSpace(oracleText))
        {
            var parseResult = oracleTextParser.Parse(oracleText, new CardContext(name));

            if (parseResult.IsFailure)
                return parseResult.ToFailure<ICardFace>();

            components.AddRange(parseResult.Value);
        }

        // Finally Create the Face
        var cardfaceres = CardFaceFactory.Create(name, typelineres.Value, oracleText, components);
        if (cardfaceres.IsFailure)
            return cardfaceres.ToFailure<ICardFace>();

        return Result<ICardFace>.Success(cardfaceres.Value);
    }

    public Result<ICard> Convert(ScryfallCard dto)
    {
        if (dto.Object != "card")
            return Result<ICard>.Failure("Object is not a card!");

        //Create Faces
        var faces = new List<ICardFace>();
        var amountFaces = dto.CardFaces == null ? 1 : dto.CardFaces.Count;
        for (var i = 0; i < amountFaces; i++)
        {
            if (dto.CardFaces == null) //Single Faced
            {
                var noFace = CreateCardFace(dto.Name, dto.TypeLine, dto.OracleText, dto.ManaCost,
                                            dto.CMC, dto.Colors, dto.ColorIndicator, dto.Power, dto.Toughness,
                                            dto.Defense, dto.Loyalty);
                if (noFace.IsFailure)
                    return noFace.ToFailure<ICard>();

                faces.Add(noFace.Value);
            }
            else //Multi Faces
            {
                var cardFace = dto.CardFaces[i];
                if (cardFace == null || cardFace.Object != "card_face")
                    return Result<ICard>.Failure("Object is not a card face!");

                var iFace = CreateCardFace(cardFace.Name, cardFace.TypeLine, cardFace.OracleText, cardFace.ManaCost,
                                           cardFace.CMC ?? -1, cardFace.Colors, cardFace.ColorIndicator,
                                           cardFace.Power, cardFace.Toughness, cardFace.Defense, cardFace.Loyalty);
                if (iFace.IsFailure)
                    return iFace.ToFailure<ICard>();

                faces.Add(iFace.Value);
            }
        }

        //Color
        var identityResult = manaParser.ParseColorStrings(dto.ColorIdentity);
        if (identityResult.IsFailure)
            return identityResult.ToFailure<ICard>();

        //Legalities
        var legalities = new Dictionary<Format, Legality>();
        foreach (var sLegality in dto.Legalities.ToList())
        {
            if (!Enum.TryParse(Conversions.ToCamelCase(sLegality.Key), out Format eFormat))
                return Result<ICard>.Failure($"Could not parse {sLegality.Key} to Format enum!");

            if (!Enum.TryParse(Conversions.ToCamelCase(sLegality.Value), out Legality eLegality))
                return Result<ICard>.Failure($"Could not parse {sLegality.Value} to Legality enum!");

            legalities.Add(eFormat, eLegality);
        }

        //Image Uris
        var imageUris = new Dictionary<ImageSize, Uri>();
        if (dto.ImageUris != null)
        {
            foreach (var sImageUri in dto.ImageUris.ToList())
            {
                if (!Enum.TryParse(Conversions.ToCamelCase(sImageUri.Key), out ImageSize eImageUri))
                    return Result<ICard>.Failure($"Could not parse {sImageUri.Key} to enum!");
                imageUris.Add(eImageUri, new Uri(sImageUri.Value));
            }
        }

        //Finally Create the Card
        var cardres = CardFactory.Create(dto.Name, dto.Set, dto.CollectorNumber, dto.TypeLine, faces,
            identityResult.Value, new Guid(dto.Id), dto.Lang, dto.Layout, dto.SetName, legalities, imageUris);
        if (cardres.IsFailure)
            return cardres.ToFailure<ICard>();
        var card = cardres.Value;

        return Result<ICard>.Success(card);
    }
}
