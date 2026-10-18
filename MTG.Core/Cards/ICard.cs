using MTG.Core.Enums;

namespace MTG.Core.Cards;

public interface ICard
{
    //Core
    Guid Id { get; }
    string Lang { get; }
    Layout Layout { get; }

    //Related Card Objects
    IReadOnlyList<CardRef> AllParts { get; }

    //Face Stuff
    IReadOnlyList<ICardFace> Faces { get; }
    ICardFace MainFace { get; }
    string MainFaceName { get; }
    string MainFaceTypeLine { get; }

    //Gameplay
    ManaType ColorIdentity { get; }
    bool IsToken { get; }
    IReadOnlyDictionary<Format, Legality> Legalities { get; }

    //Print
    string CollectorNumber { get; }
    string SetName { get; }
    string Set { get; }
    IReadOnlyDictionary<ImageSize, Uri> ImageUris { get; }
    Rarity Rarity { get; }

    //Simple Yes/No Checks
    bool IsArtifact() => MainFace.IsArtifact();
    bool IsBasic() => MainFace.IsBasic();
    bool IsBattle() => MainFace.IsBattle();
    bool IsCreature() => MainFace.IsCreature();
    bool IsHistoric() => MainFace.IsHistoric();
    bool IsInstant() => MainFace.IsInstant();
    bool IsLand() => MainFace.IsLand();
    bool IsLegendary() => MainFace.IsLegendary();
    bool IsPermanent() => MainFace.IsPermanent();
    bool IsPlaneswalker() => MainFace.IsPlaneswalker();
    bool IsMultifaced() => Faces.Count > 1;

    //ToStrings
    //string ToString();
    //string ToStringConsole();

}
