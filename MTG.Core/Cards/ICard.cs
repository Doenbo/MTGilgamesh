using MTG.Core.Enums;
using MTG.Core.Helper;
using MTG.Core.Properties;

namespace MTG.Core.Cards;

public interface ICard
{
    //Merged Face Properties
    public string FullName { get; }
    public string FullTypeLine { get; }

    //Core
    public Guid Id { get; set; }
    public string Lang { get; set; }
    public string Layout { get; set; }


    //Gameplay
    public List<ICardFace> Faces { get; }
    ICardFace MainFace => Faces[0];
    public Dictionary<Format, Legality> Legalities { get; }

    //Print
    public string CollectorNumber { get; }
    public string SetName { get; set; }
    public string Set { get; }
    public Dictionary<ImageSize, Uri> ImageUris { get; }
    public Rarity Rarity { get; }

    //Simple Getter
    public Result<ManaType> GetCardColorIdentity();

    //Simple Yes/No Checks
    public bool IsArtifact() => MainFace.IsArtifact();
    public bool IsBasic() => MainFace.IsBasic();
    public bool IsBattle() => MainFace.IsBattle();
    public bool IsCreature() => MainFace.IsCreature();
    public bool IsHistoric() => MainFace.IsHistoric();
    public bool IsInstant() => MainFace.IsInstant();
    public bool IsLand() => MainFace.IsLand();
    public bool IsLegendary() => MainFace.IsLegendary();
    public bool IsPermanent() => MainFace.IsPermanent();
    public bool IsPlaneswalker() => MainFace.IsPlaneswalker();
    public bool IsMultifaced() => Faces.Count > 1;

    //ToStrings
    public string ToString();
    public string ToStringConsole();

}
