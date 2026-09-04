using MTG.Core.Components;
using MTG.Core.Enums;
using MTG.Core.Helper;
using System.Text;

namespace MTG.Core.Cards;

public record CardCreationArgs
{
    public required string Name { get; init; }
    public required string TypeLine { get; init; }
    public required ManaType ColorIdentity { get; init; }
    public required IReadOnlyList<ICardFace> CardFaces { get; init; }
    public required IReadOnlyList<ICard> AllParts { get; init; } = [];
    public required string Set { get; init; }
    public required string CollectorNumber { get; init; }
    public required Guid Id { get; init; }
    public required string Lang { get; init; }
    public required string Layout { get; init; }
    public required string SetName { get; init; }
    public required IReadOnlyDictionary<Format, Legality> Legalities { get; init; } = new Dictionary<Format, Legality>();
    public required IReadOnlyDictionary<ImageSize, Uri> ImageUris { get; init; } = new Dictionary<ImageSize, Uri>();
}

public static class CardFactory
{
    public static Result<ICard> Create(CardCreationArgs args)
    {
        if (NullGuard.HasNullProperty(args, out var nullProperty))
            return Result<ICard>.Failure($"Card creation failed: '{nullProperty}' cannot be null!");

        if (args.CardFaces.Count == 0)        
            return Result<ICard>.Failure("A card must have at least one CardFace!");

            return Result<ICard>.Success(new Card()
        {
            Name = args.Name,
            TypeLine = args.TypeLine,
            ColorIdentity = args.ColorIdentity,
            Faces = args.CardFaces,
            AllParts = args.AllParts,
            Set = args.Set,
            CollectorNumber = args.CollectorNumber,
            Id = args.Id,
            Lang = args.Lang,
            Layout = args.Layout,
            SetName = args.SetName,
            Legalities = args.Legalities,
            ImageUris = args.ImageUris,
        });
    }

    private class Card : ICard
    {
        public Card() { }


        //Merged Face Properties
        public required string Name { get; init; }
        public required string TypeLine { get; init; }

        //Gameplay
        public required IReadOnlyList<ICardFace> Faces { get; init; } = [];
        public ICardFace MainFace => Faces[0];
        public required ManaType ColorIdentity { get; init; }
        public required IReadOnlyList<ICard> AllParts { get; init; }
        public bool IsToken { get; init; } = false;

        //Other
        public required Guid Id { get; init; }
        public required string Lang { get; init; }
        public required string Layout { get; init; }
        public IReadOnlyDictionary<Format, Legality> Legalities { get; init; }

        //Print
        public required string CollectorNumber { get; init; }
        public required string SetName { get; init; }
        public required string Set { get; init; }
        public required IReadOnlyDictionary<ImageSize, Uri> ImageUris { get; init; }
        public Rarity Rarity { get; init; }

        //ToStrings
        public override string ToString() => $"{Name} - {Set.ToUpper()}({CollectorNumber})";

        public string ToStringConsole() => MainFace.ToString();
    }
}