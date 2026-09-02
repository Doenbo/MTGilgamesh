using MTG.Core.Components;
using MTG.Core.Enums;
using MTG.Core.Helper;
using System.Text;

namespace MTG.Core.Cards;

public static class CardFactory
{
    public static Result<ICard> Create(string name, string set, string collectionnumber, string typeline)
    {
        if (name == null || set == null || collectionnumber == null)
            return Result<ICard>.Failure("Name, Set and CollectionNumber can't be null!");

        return Result<ICard>.Success(new Card()
        {
            FullName = name,
            FullTypeLine = typeline,
            Set = set,
            CollectorNumber = collectionnumber
        });
    }

    private class Card : ICard
    {
        public Card() { }


        //Merged Face Properties
        public required string FullName { get; init; }
        public required string FullTypeLine { get; init; }

        //Core
        public Guid Id { get; set; }
        public string Lang { get; set; }
        public string Layout { get; set; }

        //Gameplay
        public List<ICardFace> Faces { get; set; } = [];
        ICardFace MainFace => Faces[0];
        public Dictionary<Format, Legality> Legalities { get; set; } = [];

        //Print
        public required string CollectorNumber { get; init; }
        public string SetName { get; set; }
        public required string Set { get; init; }
        public Dictionary<ImageSize, Uri> ImageUris { get; set; } = [];
        public Rarity Rarity { get; set; }

        //Simple Getter
        public Result<ManaType> GetCardColorIdentity()
        {
            ManaType result = 0;
            foreach (var face in Faces.ToList())
            {
                if (!face.TryGetComponent<ColorComponent>(out var ident))
                    return Result<ManaType>.Failure("No Color Component?");
                result |= ident.ColorIdentity; // Bitwise Operation | means OR
            }
            return Result<ManaType>.Success(result);
        }

        //ToStrings
        public override string ToString() => $"{FullName} - {Set.ToUpper()}({CollectorNumber})";

        public string ToStringConsole() => MainFace.ToString();
    }
}