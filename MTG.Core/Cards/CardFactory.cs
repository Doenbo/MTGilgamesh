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

        return Result<ICard>.Success(new Card() { FullName = name, FullTypeLine = typeline, Set = set, CollectorNumber = collectionnumber });
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
        public List<Color> ProducedMana { get; set; } = [];
        //TODO FullOracleText ?

        //Print
        public required string CollectorNumber { get; init; }
        public string SetName { get; set; }
        public required string Set { get; init; }
        public Dictionary<ImageSize, Uri> ImageUris { get; set; } = [];
        public Rarity Rarity { get; set; }

        //Simple Getter
        public Result<Color> GetCardColorIdentity()
        {
            Color result = 0;
            foreach (var face in Faces)
            {
                if (!face.TryGetComponent<ColorComponent>(out var ident))
                    return Result<Color>.Failure("No Color Component?");
                result |= ident.ColorIdentity; // Bitwise Operation | mean OR
            }
            return Result<Color>.Success(result);
        }

        //ToStrings
        public override string ToString() => $"{FullName} - {Set.ToUpper()}({CollectorNumber})";

        public string ToStringConsole()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"/------------------------------------\\");
            if (MainFace.TryGetComponent<ManaComponent>(out var manaComp))
            {
                sb.AppendLine($"|{MainFace.Name} {manaComp.ManaCost.ToString()}");
            }
            else
            {
                sb.AppendLine($"|{MainFace.Name}");
            }

            sb.AppendLine($"|{MainFace.TypeLine}");

            //TODO auslagern?
            var threewords = string.Empty;
            if (MainFace.KeywordAbilities.Count != 0)
            {
                var keywords = string.Concat(MainFace.KeywordAbilities.Select(item => $"{{{item}}}"));
                threewords += keywords;
            }

            if (MainFace.KeywordActions.Count != 0)
            {
                var keywords = string.Concat(MainFace.KeywordActions.Select(item => $"{{{item}}}"));
                threewords += keywords;
            }

            if (MainFace.AbilityWords.Count != 0)
            {
                var keywords = string.Concat(MainFace.AbilityWords.Select(item => $"{{{item}}}"));
                threewords += keywords;
            }
            if (threewords != string.Empty)
                sb.AppendLine($"|{threewords}");

            if (!string.IsNullOrEmpty(MainFace.OracleText))
            {
                var oracletext = MainFace.OracleText.Replace("\n", $"{Environment.NewLine}|");
                sb.AppendLine($"|{oracletext}");
            }

            if (MainFace.TryGetComponent<CreatureComponent>(out var creatureComp))
            {
                sb.AppendLine($"|{Set.ToUpper()}({CollectorNumber}) - ({creatureComp.Power}/{creatureComp.Toughness})");
            }
            else
            {
                sb.AppendLine($"|{Set.ToUpper()}({CollectorNumber})");
            }

            sb.AppendLine($"\\------------------------------------/");

            return sb.ToString();
        }
    }
}