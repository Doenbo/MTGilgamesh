using MTG.Core.Components;
using MTG.Core.Enums;
using MTG.Core.Helper;
using System.Text;

namespace MTG.Core.Cards;

public static class CardFactory
{
    public static Result<ICard> Create(string name, string set, string collectionNumber, string typeLine,
        List<ICardFace> cardfaces, ManaType colorIdentity, Guid id, string lang, string layout, string setName,
        Dictionary<Format, Legality> legalities, Dictionary<ImageSize, Uri> imageUris)
    {
        if (name == null || set == null || collectionNumber == null)
            return Result<ICard>.Failure("Name, Set and CollectionNumber can't be null!");

        return Result<ICard>.Success(new Card()
        {
            FullName = name,
            FullTypeLine = typeLine,
            Set = set,
            CollectorNumber = collectionNumber,
            Faces = cardfaces,
            ColorIdentity = colorIdentity,
            Id = id,
            Lang = lang,
            Layout = layout,
            SetName = setName,
            Legalities = legalities,
            ImageUris = imageUris,
        });
    }

    private class Card : ICard
    {
        public Card() { }


        //Merged Face Properties
        public required string FullName { get; init; }
        public required string FullTypeLine { get; init; }

        //Gameplay
        public IReadOnlyList<ICardFace> Faces { get; init; } = [];
        ICardFace MainFace => Faces[0];
        public required ManaType ColorIdentity { get; init; }
        public bool IsToken { get; init; } = false;

        //Other
        public required Guid Id { get; init; }
        public required string Lang { get; init; }
        public required string Layout { get; init; }
        public Dictionary<Format, Legality> Legalities { get; init; } = [];

        //Print
        public required string CollectorNumber { get; init; }
        public required string SetName { get; init; }
        public required string Set { get; init; }
        public required Dictionary<ImageSize, Uri> ImageUris { get; init; } = [];
        public Rarity Rarity { get; init; }

        //ToStrings
        public override string ToString() => $"{FullName} - {Set.ToUpper()}({CollectorNumber})";

        public string ToStringConsole() => MainFace.ToString();
    }
}